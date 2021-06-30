using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using esp.xr.Tools.EnumeratorHelper;

/*
有限状态机
通过状态枚举类型和事件类型参数化
状态通过动作和可选的转换来响应事件
无法在运行时配置
*/


namespace xr.SetpByStep.FSM
{
    struct FastEnumIntEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct
    {
        public int ToInt(TEnum en)
        {
            return EnumInt32ToInt.Convert(en);
        }

        public bool Equals(TEnum firstEnum, TEnum secondEnum)
        {
            return ToInt(firstEnum) == ToInt(secondEnum);
        }

        public int GetHashCode(TEnum firstEnum)
        {
            return ToInt(firstEnum);
        }

        public int Compare(TEnum firstEnum, TEnum secondEnum)
        {
            int result = ToInt(firstEnum) - ToInt(secondEnum);
            return result == 0 ? 0 : (result > 0 ? -1 : 1);
        }
    }

    public class FSM<TState, TEvent>
        where TState : struct, IComparable, IConvertible, IFormattable
        where TEvent : struct, IComparable, IConvertible, IFormattable
    {
        private struct EventHandler
        {
            public EventHandler(Action reaction, TState transitionTarget)
            {
                m_reaction = reaction;
                m_transitionTarget = transitionTarget;
            }

            public void React()
            {
                if (null != m_reaction)
                {
                    m_reaction();
                }
            }

            public bool IsNull()
            {
                return null == m_reaction && StateEquals(m_transitionTarget, default(TState));
            }

            public Action m_reaction;
            public TState m_transitionTarget;
        }

        private struct FSMState
        {
            public TState m_state;

            public Action m_onEnter;
            public Action m_onExit;
            public Action m_onUpdate;
            public Dictionary<TEvent, EventHandler> m_reactions;

            public void Init(TState state)
            {
                m_state = state;
                m_onEnter = null;
                m_onExit = null;
                m_onUpdate = null;
                m_reactions = new Dictionary<TEvent, EventHandler>();
            }

            public bool IsValid()
            {
                return null != m_onEnter ||
                       null != m_onExit ||
                       null != m_onUpdate ||
                       m_reactions.Count > 0;
            }

            public void Enter()
            {
                if (null != m_onEnter)
                {
                    m_onEnter();
                }
            }

            public void Exit()
            {
                if (null != m_onExit)
                {
                    m_onExit();
                }
            }

            public void Update()
            {
                if (null != m_onUpdate)
                {
                    m_onUpdate();
                }
            }

            public bool TryGetReaction(TEvent evt, out EventHandler reaction)
            {
                return m_reactions.TryGetValue(evt, out reaction);
            }
        }

        private static readonly Type m_stateType = typeof(TState);
        private static readonly TState INITIAL_STATE = default(TState);

        private static readonly FastEnumIntEqualityComparer<TState> s_stateComparator =
            new FastEnumIntEqualityComparer<TState>();

        private static readonly Type m_eventType = typeof(TEvent);
        private static readonly TEvent START_EVENT = default(TEvent);

        private static readonly string s_name = "FSM<" + m_stateType.Name + "," + m_eventType.Name + ">";

        private bool m_runState = false;
        private bool m_reactionGuard = false;

        private TState m_currentState = INITIAL_STATE;
        private TState m_previousState = INITIAL_STATE;

        private FSMState[] m_states;
        private Queue<TEvent> m_queuedEvents = new Queue<TEvent>();

        public const string INITIAL_STATE_NAME = "INITIAL";
        public const string START_EVENT_NAME = "START";

        public bool IsRunning()
        {
            return m_runState;
        }

        public FSM()
        {
            Debug.Assert(m_stateType.IsEnum, "用非枚举状态类实例化的FSM: " + m_stateType.Name);
            var stateNames = Enum.GetNames(m_stateType);
            Debug.Assert(stateNames.Length > 0, "FSM实例化了无效的空状态枚举: " + m_stateType.Name);
            Debug.Assert(stateNames.First() == INITIAL_STATE_NAME,
                "FSM实例化了无效的状态枚举 " + m_stateType.Name + ": 第一个状态不是 \"" +
                INITIAL_STATE_NAME + "\"");

            Debug.Assert(m_eventType.IsEnum, "用非枚举状态类实例化的FSM: " + m_eventType.Name);
            var eventNames = Enum.GetNames(m_eventType);
            Debug.Assert(eventNames.Length > 0, "FSM实例化了无效的空状态枚举: " + m_eventType.Name);
            Debug.Assert(eventNames.First() == START_EVENT_NAME,
                "FSM实例化了无效的状态枚举 " + m_eventType.Name + ": 第一个状态不是 \"" +
                START_EVENT_NAME + "\"");


            var values = Enum.GetValues(m_stateType);
            m_states = new FSMState[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var value = values.GetValue(i);
                Debug.Assert(i == (int) value,
                    "FSM用无效枚举实例化 " + m_stateType.Name + ": 无效的不连续的值: " +
                    value + " for state: " + stateNames[i]);
                TState state = (TState) value;
                m_states[i].Init(state);
            }
        }


        public void SetOnEnter(TState state, Action action)
        {
            Debug.Assert(!m_runState, s_name + ".SetOnEnter() called while running");
            Debug.Assert(null != action, s_name + ".SetOnEnter(): invalid action for state: " + ToString(state));
            Debug.Assert(null == m_states[ToIndex(state)].m_onEnter,
                s_name + ".SetOnEnter(): OnEnter already set for state: " + ToString(state));
            m_states[ToIndex(state)].m_onEnter = action;
        }

        public void SetOnExit(TState state, Action action)
        {
            Debug.Assert(!m_runState, s_name + ".SetOnExit() called while running");
            Debug.Assert(null != action, s_name + ".SetOnExit(): invalid action for state: " + ToString(state));
            Debug.Assert(null == m_states[ToIndex(state)].m_onExit,
                s_name + ".SetOnExit(): OnExit already set for state: " + ToString(state));
            m_states[ToIndex(state)].m_onExit = action;
        }

        public void SetOnUpdate(TState state, Action action)
        {
            Debug.Assert(!m_runState, s_name + ".SetOnUpdate() called while running");
            Debug.Assert(null != action, s_name + ".SetOnUpdate(): invalid action for state: " + ToString(state));
            Debug.Assert(null == m_states[ToIndex(state)].m_onUpdate,
                s_name + ".SetOnUpdate(): OnUpdate already set for state: " + ToString(state));
            m_states[ToIndex(state)].m_onUpdate = action;
        }

        // Adding a reaction with a transtion to TEvent.INITIAL
        // means no transition (pure reaction)
        // adding a reaction with a transition from state to itself
        // means an actual transition, i.e. exiting the current, executing the reaction, re-entering the state
        public void SetReaction(TState state, TEvent evt, TState newState = default(TState), Action action = null)
        {
            Debug.Assert(!m_runState, s_name + ".SetReaction() called while running");
            Debug.Assert(!StateEquals(newState, INITIAL_STATE) || null != action,
                s_name + ".SetReaction: invalid empty reaction - state: " + ToString(state) + " - event: " +
                ToString(evt));
            Debug.Assert(!m_states[ToIndex(state)].m_reactions.ContainsKey(evt),
                s_name + ".SetReaction: state " + ToString(state) + " already contains a reaction to " + ToString(evt));
            m_states[ToIndex(state)].m_reactions[evt] = new EventHandler(action, newState);
        }



        public void Start()
        {
            Debug.Assert(!m_runState, s_name + ".Start() called while running");

#if ASSERTS
		for (int i = 0; i < m_states.Length; ++i)
		{
			Debug.Assert(m_states[i].IsValid(), s_name + " has invalid FSM state: " + m_states[i].m_state);
		}
#endif

            m_currentState = INITIAL_STATE;
            m_previousState = INITIAL_STATE;
            m_runState = true;
            ReactTo(START_EVENT);
        }

        public void Stop()
        {
            Debug.Assert(m_runState, s_name + ".Stop() called while not running");

            m_runState = false;
        }

        public void Update()
        {
            if (m_runState)
            {
                m_states[ToIndex(m_currentState)].Update();
            }
        }

        public bool ReactTo(TEvent evt)
        {
            Debug.Assert(m_runState, s_name + ".ReactTo() called while not running");

            var reacted = false;
            // if we're already reacting to an event
            // we queue the event to be processed right after
            // this allows events to be sent as reactions to events
            // or entering / exiting states
            if (m_reactionGuard)
            {
                m_queuedEvents.Enqueue(evt);
            }
            else
            {
                reacted = DoReactTo(evt);
            }

            return reacted;
        }


        private bool DoReactTo(TEvent evt)
        {
            Debug.Assert(m_runState, s_name + ".DoReactTo() called while running");
            var reacted = ProcessEvent(evt);
            while (0 != m_queuedEvents.Count)
            {
                var queuedEvent = m_queuedEvents.Dequeue();
                reacted = ProcessEvent(queuedEvent);
            }

            return reacted;
        }

        private bool ProcessEvent(TEvent evt)
        {
            Debug.Assert(m_runState, s_name + ".ReactTo() called while running");
            Debug.Assert(!m_reactionGuard,
                s_name + " .ReactTo() " + evt +
                " - already reacting to another event - chained reactions are not allowed");
            m_reactionGuard = true;
            EventHandler reaction;
            int stateIndex = ToIndex(m_currentState);
            bool hasReaction = m_states[stateIndex].TryGetReaction(evt, out reaction);
            if (hasReaction)
            {
                // transitions to initial state are illegal and
                // used to mark non-transitions
                bool hasTransition = !StateEquals(reaction.m_transitionTarget, INITIAL_STATE);
                if (hasTransition)
                {
                    m_previousState = m_currentState;
                    m_currentState = reaction.m_transitionTarget;
                    Debug.Log("<color=cyan>" + s_name + " exiting state: " + m_previousState + " from event: " + evt +
                               "</color>");
                    m_states[stateIndex].Exit();
                }

                reaction.React();

                if (hasTransition)
                {
                    Debug.Log("<color=cyan>" + s_name + " entering state: " + m_currentState + " from event: " + evt +
                               "</color>");
                    m_states[ToIndex(m_currentState)].Enter();
                }
            }

            m_reactionGuard = false;
            Debug.Assert(hasReaction,
                s_name + " has no reaction to event: " + ToString(evt) + " from state: " + ToString(m_currentState));
            return hasReaction;
        }

        public TState CurrentState
        {
            get { return m_currentState; }
        }

        public TState PreviousState
        {
            get { return m_previousState; }
        }

        public bool IsInState(TState state)
        {
            var result = StateEquals(CurrentState, state);
            return result;
        }


        static public int ToIndex(TState state)
        {
            return s_stateComparator.ToInt(state);
        }

        static public bool StateEquals(TState lhs, TState rhs)
        {
            var result = s_stateComparator.Equals(lhs, rhs);
            return result;
        }


        // we need to define these proxies because
        // Enum to string conversion is not allowed on IPhone
        static private string ToString(TState state)
        {
#if UNITY_IPHONE
		return "n/a";
#else
            return state.ToString();
#endif
        }

        static private string ToString(TEvent evt)
        {
#if UNITY_IPHONE
		return "n/a";
#else
            return evt.ToString();
#endif
        }

        // can't have this because C# doesn't support references to structs
        // i.e. this returns a copy of the state, not the actual state
        //	private FSMState GetFSMState(TState state)
        //	{
        //		return m_states[(int)state];
        //	}
        //
        //	private FSMState GetCurrentFSMState()
        //	{
        //		return GetFSMState(m_currentState);
        //	}



    }
}