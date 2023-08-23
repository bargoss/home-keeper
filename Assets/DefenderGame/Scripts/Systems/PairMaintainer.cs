using System;
using System.Collections.Generic;
using System.Linq;

namespace DefenderGame.Scripts.Systems
{
    public class PairMaintainer<TLogical, TView>
    {
        private readonly Dictionary<TLogical, TView> m_LogicalToView = new();
        private readonly HashSet<TLogical> m_Touched = new();
        
        private readonly Func<TLogical, TView> m_CreateView;
        private readonly Action<TView> m_DisposeView;
        
        //ctor
        public PairMaintainer(Func<TLogical, TView> createView, Action<TView> disposeView)
        {
            m_CreateView = createView;
            m_DisposeView = disposeView;
        }
        
        public TView GetOrCreateView(TLogical logical)
        {
            m_Touched.Add(logical);
            if (m_LogicalToView.TryGetValue(logical, out var view))
            {
                return view;
            }
            else
            {
                var pair = m_CreateView(logical);
                m_LogicalToView.Add(logical, pair);
                return pair;
            }
        }

        public int DisposeAndClearUntouchedViews()
        {
            int disposedCount = 0;
            foreach (var logical in m_LogicalToView.Keys.ToList())
            {
                if (!m_Touched.Contains(logical))
                {
                    m_DisposeView(m_LogicalToView[logical]);
                    m_LogicalToView.Remove(logical);
                    disposedCount++;
                }
            }
            m_Touched.Clear();
            
            return disposedCount;
        }
    }
}