﻿using System.Collections.Generic;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts;

namespace Planner
{
    internal interface IActualPlannerBrain
    {
        bool IsNewPlanAvailable();
        List<Action> GetNewPlan();
        ASPExecutor GetPlannerExecutor(PlannerBrain plannerBrain);
        void NewPlanAvailable(object plan);
        string ActualSensorEncoding(string sensorsAsASP);
        string SpecificFileParts();
    }
}