﻿using System;
using SpaceSim.Contracts.Commands;
using SpaceSim.Engines;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    [Serializable]
    class ShutdownCommand : CommandBase
    {
        private double _currentThrottle;
        private int[] _engineIds;

        public ShutdownCommand(Shutdown shutdown)
            : base(shutdown.StartTime, shutdown.Duration)
        {
            _engineIds = shutdown.EngineIds;
        }

        public override void Initialize(ISpaceCraft spaceCraft)
        {
            if (_engineIds == null)
            {
                EventManager.AddMessage("Shutting down all engines", spaceCraft);
            }
            else
            {
                EventManager.AddMessage(string.Format("Shutting down {0} engines", _engineIds.Length), spaceCraft);
            }


            _currentThrottle = spaceCraft.Throttle;
        }

        public override void Finalize(ISpaceCraft spaceCraft)
        {
            // Shutdown all engines
            if (_engineIds == null)
            {
                foreach (IEngine engine in spaceCraft.Engines)
                {
                    engine.Shutdown();
                }
            }
            else
            {
                // Shut down specific engines
                foreach (int engineId in _engineIds)
                {
                    spaceCraft.Engines[engineId].Shutdown();
                }
            }
        }

        public override void Update(double elapsedTime, ISpaceCraft spaceCraft)
        {
            double shutdownRatio = (elapsedTime - StartTime) * 2;

            spaceCraft.SetThrottle(_currentThrottle * (1 - shutdownRatio), _engineIds);
        }
    }
}
