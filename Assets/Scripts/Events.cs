using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Tin.Events
{
    public class Listener
    {
        public System.Delegate _delegate;
        public System.Type _type;
        public int _priority;
        public Listener(System.Type t, System.Delegate d, int p)
        {
            _delegate = d;
            _type = t;
            _priority = p;
        }
    }

    public class RoutineListener<A>
    {
        public Func<A, IEnumerator> _delegate;
        public System.Type _type;
        public int _priority;
        public RoutineListener(System.Type t, Func<A, IEnumerator> d, int p)
        {
            _delegate = d;
            _type = t;
            _priority = p;
        }
    }

    public class EventRoutine<T>
    {
        interface IEventRoutine
        {
            void Add(object item);
            void Add(object item, int priority);
            void Detach(object item);
            bool parallel { get; set; }
        }
        public delegate IEnumerator EventRoutine_delegate<A>(A args);
        public class RoutineListContainer<A> : List<RoutineListener<A>>, IEventRoutine where A : T
        {
            public void Add(object item)
            {
                base.Add((RoutineListener<A>)item);
            }
            public void Add(object item, int priority)
            {
                base.Add((RoutineListener<A>)item);
            }
            public void Detach(object item)
            {
                base.Remove(item as RoutineListener<A>);
            }
            public bool parallel { get; set; }
        }
        public class ActionListContainer<A> : List<Listener>, IEventRoutine
        {
            public void Add(object item)
            {
                base.Add((Listener)item);
            }
            public void Add(object item, int priority)
            {
                base.Add((Listener)item);
            }
            public void Detach(object item)
            {
                base.Remove(item as Listener);
            }
            public bool parallel { get; set; }
        }
        private Dictionary<System.Type, IEventRoutine> routines = new Dictionary<System.Type, IEventRoutine>();
        private Dictionary<System.Type, IEventRoutine> actions = new Dictionary<System.Type, IEventRoutine>();
        public IEnumerator Post<A>(A _args) where A : T
        {
            if (actions != null && actions.ContainsKey(typeof(A)))
            {
                ActionListContainer<A> action_list = actions[typeof(A)] as ActionListContainer<A>;

                List<Listener> temp = new List<Listener>(action_list);
                for (int i = 0; i < temp.Count; i++)
                {
                    Listener it = temp[i];
                    if (it == null) continue;
                    (it._delegate as System.Action<A>)(_args);
                }
            }

            if (routines != null && routines.ContainsKey(typeof(A)))
            {
                RoutineListContainer<A> routine_list = routines[typeof(A)] as RoutineListContainer<A>;
                //Extensions.RunInfo inf = null;
                List<RoutineListener<A>> temp = new List<RoutineListener<A>>(routine_list);
                for (int i = 0; i < temp.Count; i++)
                {
                    RoutineListener<A> it = temp[i];
                    if (it == null) continue;
/* 
                    if (routine_list.parallel)
                    {
                        inf = it._delegate(_args).ParallelCoroutine(typeof(A).ToString());
                    }
                    else  */
                    yield return it._delegate(_args);
                }

                /* if (routine_list.parallel)
                {
                    while (inf.count > 0) yield return null;
                } */
            }
        }

        public RoutineListener<A> Attach<A>(Func<A, IEnumerator> ienum) where A : T
        {
            return Attach<A>(10, ienum);
        }

        public RoutineListener<A> Attach<A>(int priority, Func<A, IEnumerator> ienum) where A : T
        {
            //TODO: check if this routine has been attached already
            if (!routines.ContainsKey(typeof(A)))
            {
                routines[typeof(A)] = new RoutineListContainer<A>();
            }
            if((routines[typeof(A)] as RoutineListContainer<A>).Find(c=>c._delegate == ienum) != null) return (routines[typeof(A)] as RoutineListContainer<A>).Find(c=>c._delegate == ienum);
            
            RoutineListener<A> final = new RoutineListener<A>(typeof(A), ienum, priority);
            routines[typeof(A)].Add(final, priority);
            
            return final;
        }

        public void Detach<A>(RoutineListener<A> listener) where A : T
        {
            if (!routines.ContainsKey(listener._type)) return;
            routines[listener._type].Detach(listener._delegate);
        }


        /* public Func<A, IEnumerator> Attach<A>(Func<A, IEnumerator> ienum) where A : T
        {
            //TODO: check if this routine has been attached already
            if (!routines.ContainsKey(typeof(A)))
            {
                routines[typeof(A)] = new RoutineListContainer<A>();
            }
            routines[typeof(A)].Add(ienum);
            return ienum;
        } */


        /* public void Detach<A>(Func<A, IEnumerator> ienum)  where A : T
        {
            if (!routines.ContainsKey(typeof(A))) return;
            routines[typeof(A)].Detach(ienum);
        } */
        public Listener Attach<A>(Action<A> func) where A : T
        {
            return Attach<A>(10, func);
        }
        public Listener Attach<A>(int priority, Action<A> func) where A : T
        {
            if (!actions.ContainsKey(typeof(A)))
            {
                actions[typeof(A)] = new ActionListContainer<A>();
            }
            Listener final = new Listener(typeof(A), func, priority);
            actions[typeof(A)].Add(final, priority);
            return final;
        }
        public void Detach(Listener listener)
        {
            if (listener == null) return;
            if (!actions.ContainsKey(listener._type)) return;
            actions[listener._type].Detach(listener);
        }

        public void SetParallel<A>(bool p = true) where A : T
        {
            if (!routines.ContainsKey(typeof(A)))
            {
                routines[typeof(A)] = new RoutineListContainer<A>();
            }
            routines[typeof(A)].parallel = p;
        }

        /* public static T Cast<T>(Delegate source) where T : class
        {
            return Cast(source, typeof(T)) as T;
        }
        public static Delegate Cast(Delegate source, Type type)
        {
            if (source == null)
                return null;
            Delegate[] delegates = source.GetInvocationList();
            if (delegates.Length == 1)
                return Delegate.CreateDelegate(type,
                    delegates[0].Target, delegates[0].Method);
            Delegate[] delegatesDest = new Delegate[delegates.Length];
            for (int nDelegate = 0; nDelegate < delegates.Length; nDelegate++)
                delegatesDest[nDelegate] = Delegate.CreateDelegate(type,
                    delegates[nDelegate].Target, delegates[nDelegate].Method);
            return Delegate.Combine(delegatesDest);
        } */
    }
}

