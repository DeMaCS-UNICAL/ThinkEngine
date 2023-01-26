namespace it.unical.mat.asp_classes
{
	using Id = it.unical.mat.embasp.languages.Id;
	using Param = it.unical.mat.embasp.languages.Param;

[Id("connected")]
	public class Connected
	{
    [Param(0)]
		private string partition1;
		
    [Param(1)]
    private string partition2;

		public Connected() : base()
		{
		}

		public Connected(string partition1, string partition2) : base()
		{
			this.partition1 = partition1;
			this.partition2 = partition2;
		}

    public virtual string getPartition1() {
      return partition1;
    }

    public virtual void setPartition1(string value) {
      this.partition1 = value;
    }

    public virtual string getPartition2() {
      return partition2;
    }

    public virtual void setPartition2(string value) {
      this.partition2 = value;
    }



    public override string ToString()
		{
			return "connected(" + partition1 + "," + partition2 + ").";
		}

	}

}