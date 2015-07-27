using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FirePrize
{
    public class FirebaseTrackedObject<T>
    {
        public string Path { get; set; }
        public T Object { get; set; }
    }

    public class FireList<T> : IEnumerable<T>, INotifyCollectionChanged where T : new()
    {
        private static char[] PathSplitChars = new[] { '/' };

        private IFirebaseClient firebase;
        private string name;
        private List<FirebaseTrackedObject<T>> list;
        private object lockObject = new object();

        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

        public FireList(IFirebaseClient firebase, string name)
            : base()
        {
            this.list = new List<FirebaseTrackedObject<T>>();

            this.firebase = firebase;
            this.name = name;

            ValueRootAddedEventHandler<T> addedHandler = (s, obj) =>
            {
                Console.WriteLine("FOO!");
            };
            firebase.OnChangeGetAsync(name, addedHandler);

            firebase.OnAsync(name, (s, a) =>
            {
                // If there are multiple slashes, we are setting a property of one of our objects.
                if (a.Path.LastIndexOf('/') > 0)
                {
                    // Split on slashes to find out if this is an object property, or sub-object property...
                    var sections = a.Path.Split(PathSplitChars, StringSplitOptions.RemoveEmptyEntries);
                    if (sections.Length == 2)
                    {
                        // If there is not an object yet matching this path, make one to match it.
                        // Locks to prevent multiple callbacks from creating duplicates from the same object path.
                        var path = "/" + sections[0];
                        FirebaseTrackedObject<T> tracked = null;
                        lock (this.lockObject)
                        {
                            tracked = (from t in this.list where t.Path == path select t).FirstOrDefault();
                            if (tracked == null)
                            {
                                tracked = new FirebaseTrackedObject<T>()
                                {
                                    Object = new T(),
                                    Path = path,
                                };

                                this.list.Add(tracked);

                                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tracked.Object);
                                Application.Current.Dispatcher.BeginInvoke(new Action(() => this.CollectionChanged(this, args)));
                            }
                        }

                        // Use reflection to locate the described property, and set it to the described value.  This relies
                        // on Convert.ChangeType, which does not support converting strings to every type in existence, so
                        // if an exception is thrown here, you may need a custom converter.  GetProperty may return null if
                        // the code has changed to remove the property of an object that is still tracked in Firebase; check
                        // for null to be safe. TODO: Consider writing warning output.
                        // TODO: Consider adding something like a SetCustomConverter<T>(...), which wins over ChangeType.
                        var prop = typeof(T).GetProperty(sections[1]);
                        if (prop != null)
                        {
                            if (prop.PropertyType == typeof(Guid))
                            {
                                prop.SetValue(tracked.Object, new Guid(a.Data));
                            }
                            else
                            {
                                prop.SetValue(tracked.Object, Convert.ChangeType(a.Data, prop.PropertyType));
                            }
                        }
                    }
                }
                Console.WriteLine("ADD!");
            }, (s, a) =>
            {
                Console.WriteLine("CHANGE!");
            }, (s, a) =>
            {
                Console.WriteLine("REMOVE!");
            });
        }

        public ReadOnlyCollection<T> Items
        {
            get { return this.list.Select(o => o.Object).ToList().AsReadOnly(); }
        }

        public async void Add(T obj)
        {
            var trackedObj = new FirebaseTrackedObject<T>()
            {
                Object = obj,
            };
            var response = await this.firebase.PushAsync(this.name, obj);
            trackedObj.Path = response.Result.Name;

            // NOW RELY ON CALLBACK
            //var oldItems = this.Items;
            //this.list.Add(trackedObj);
            //var newItems = this.Items;
            //
            //this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj));
        }

        public async void Remove(T obj)
        {
            var item = (from t in this.list where t.Object.Equals(obj) select t).FirstOrDefault();
            if (item != null)
            {
                await this.firebase.DeleteAsync(item.Path);

                this.list.Remove(item);
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
    }
}