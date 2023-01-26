namespace it.unical.mat.asp_classes
{
	using UnavCells = it.unical.mat.debug.UnavCells;
	using Handler = it.unical.mat.embasp.@base.Handler;
	using InputProgram = it.unical.mat.embasp.@base.InputProgram;
	using OptionDescriptor = it.unical.mat.embasp.@base.OptionDescriptor;
	using ASPInputProgram = it.unical.mat.embasp.languages.asp.ASPInputProgram;
	using ASPMapper = it.unical.mat.embasp.languages.asp.ASPMapper;
	using DesktopHandler = it.unical.mat.embasp.platforms.desktop.DesktopHandler;
	using ClingoDesktopService = it.unical.mat.embasp.specializations.clingo.desktop.ClingoDesktopService;
  using AIMapGenerator;

  public class EmbASPManager
	{
		private readonly bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			handler = new DesktopHandler(new ClingoDesktopService(solverPath));
		}


		private readonly string solverPath = MapGeneratorLayout.solver;
		private Handler handler;
		private InputProgram input = new ASPInputProgram();

		public EmbASPManager()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public EmbASPManager(Handler handler, string solverPath, InputProgram input) : base()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.handler = handler;
			this.solverPath = solverPath;
			this.input = input;
		}

		public EmbASPManager(InputProgram input) : base()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.input = input;
		}

    public virtual Handler Handler => handler;

    public virtual InputProgram Input => input;

    public virtual void InitializeEmbASP()
		{
			ASPMapper.Instance.RegisterClass(typeof(Cell));
			ASPMapper.Instance.RegisterClass(typeof(NewDoor));
			ASPMapper.Instance.RegisterClass(typeof(UnavCells));
			ASPMapper.Instance.RegisterClass(typeof(Connected8));
			ASPMapper.Instance.RegisterClass(typeof(Partition));
			ASPMapper.Instance.RegisterClass(typeof(Assignment));
			ASPMapper.Instance.RegisterClass(typeof(ObjectAssignment));
		}

		public virtual void InitializeEmbASP(int randomAnswersetNumber)
		{
			InitializeEmbASP();
			// Initialize the options for the solver
			handler.AddOption(new OptionDescriptor("-n" + randomAnswersetNumber));
	//        handler.addOption(new OptionDescriptor(" --filter=cell/3,new_door/3,object_assignment6/6,connected8/8,assignment5/5,partition4/4"));
		}
	}

}