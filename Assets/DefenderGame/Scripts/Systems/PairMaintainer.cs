using System;
using System.Collections.Generic;

namespace DefenderGame.Scripts.Systems
{
    public class PairMaintainer<TLogical, TView>
    {
        private Dictionary<TLogical, TView> LogicalToView = new();
        private HashSet<TLogical> m_Touched = new();
        
        private Func<TLogical, TView> m_CreateView;
        private Action<TView> m_DisposeView;
        
        //ctor
        public PairMaintainer(Func<TLogical, TView> createView, Action<TView> disposeView)
        {
            m_CreateView = createView;
            m_DisposeView = disposeView;
        }
        
        public TView GetOrCreateView(TLogical logical)
        {
            m_Touched.Add(logical);
            if (LogicalToView.TryGetValue(logical, out var view))
            {
                return view;
            }
            else
            {
                var pair = m_CreateView(logical);
                LogicalToView.Add(logical, pair);
                return pair;
            }
        }

        public void DisposeAndClearUntouchedViews()
        {
            foreach (var logical in LogicalToView.Keys)
            {
                if (!m_Touched.Contains(logical))
                {
                    m_DisposeView(LogicalToView[logical]);
                    LogicalToView.Remove(logical);
                }
            }
            m_Touched.Clear();
        }
    }
}