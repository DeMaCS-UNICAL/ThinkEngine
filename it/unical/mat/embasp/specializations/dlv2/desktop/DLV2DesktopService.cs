using it.unical.mat.embasp.@base;
using System.Collections.Generic;

namespace it.unical.mat.embasp.specializations.dlv2.desktop
{
    using Output = it.unical.mat.embasp.@base.Output;
    using DesktopService = it.unical.mat.embasp.platforms.desktop.DesktopService;

    public class DLV2DesktopService : DesktopService
    {
        private OptionDescriptor competitionOutputOption;

        public DLV2DesktopService(string exe_path) : base(exe_path)
        {
            load_from_STDIN_option = "--stdin";
            competitionOutputOption = new OptionDescriptor("--competition-output");
        }

        protected internal override Output GetOutput(string output, string error) => new DLV2AnswerSets(output, error);
        public override Output StartSync(IList<InputProgram> programs, IList<OptionDescriptor> options)
        {
            return base.StartSync(programs, options);
        }
    }
}