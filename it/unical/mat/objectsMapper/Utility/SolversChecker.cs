using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.specializations.clingo.desktop;
using it.unical.mat.embasp.specializations.dlv2.desktop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThinkEngine
{
    internal static class SolversChecker
    {

        internal static Handler GetHandler(Brain brain)
        {
            foreach (string fileName in Directory.GetFiles(Path.Combine(Utility.StreamingAssetsContent, "lib")))
            {
                string actualFileName = fileName.Substring(fileName.LastIndexOf(Utility.slash) + 1);
                if (actualFileName.StartsWith(brain.SolverName)  && actualFileName.EndsWith(Utility.RunnableExtension))
                {
                    if (brain.SolverName.Equals("dlv"))
                    {
                        return new DesktopHandler(new DLV2DesktopService(fileName));
                    }
                    else if (brain.SolverName.Equals("clingo"))
                    {
                        return new DesktopHandler(new ClingoDesktopService(fileName));
                    }
                }
            }
            throw new Exception("Unable to find ASP solver");
        }

        internal static string GetSolverName(int solverIndex)
        {
            switch (solverIndex)
            {
                case 0: return "clingo";
                case 1: return "dlv";
            }
            throw new Exception("Solver not supported");
        }

        internal static List<string> AvailableSolvers()
        {
            List<string> availableSolvers = new List<string>();
            foreach (string fileName in Directory.GetFiles(Path.Combine(Utility.StreamingAssetsContent, "lib")))
            {
                string actualFileName = fileName.Substring(fileName.LastIndexOf(Utility.slash) + 1);
                if (actualFileName.StartsWith("dlv") && actualFileName.EndsWith(Utility.RunnableExtension))
                {
                    availableSolvers.Add("DLV");
                }
                if (actualFileName.StartsWith("clingo") && actualFileName.EndsWith(Utility.RunnableExtension))
                {
                    availableSolvers.Add("Clingo");
                }
            }
            availableSolvers.Sort();
            return availableSolvers;
        }
        internal static bool CheckSolver(string solverName)
        {
            string[] libContent = Directory.GetFiles(Path.Combine(Utility.StreamingAssetsContent, "lib"));
            if (libContent.Length == 0)
            {
                return false;
            }
            foreach (string filename in libContent)
            {
                string actualFileName = filename.Substring(filename.LastIndexOf(Utility.slash) + 1);
                if ((actualFileName.StartsWith("dlv") || actualFileName.StartsWith("clingo")) && actualFileName.EndsWith(Utility.RunnableExtension))
                {
                    return true;
                }
            }
            return false;
            throw new NotImplementedException();
        }
    }
}
