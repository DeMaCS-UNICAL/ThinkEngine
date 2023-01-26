using it.unical.mat.asp_classes;
using it.unical.mat.map_generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIMapGenerator.it.unical.mat.asp_classes {

  public abstract class BaseThread {
    private Thread _thread;

    protected BaseThread() {
      _thread = new Thread(new ThreadStart(this.RunThread));
    }

    // Thread methods / properties
    public void Start() => _thread.Start();
    public void Join() => _thread.Join();
    public bool IsAlive => _thread.IsAlive;

    // Override in base class
    public abstract void RunThread();
  }

  public class RoomBuilder : BaseThread {

    private MapGenerator mapGenerator;
    private Partition partition;
    private Barrier barrier;

    public RoomBuilder(MapGenerator mapGenerator, Partition partition, Barrier barrier) {
      this.mapGenerator = mapGenerator;
      this.partition = partition;
      this.barrier = barrier;
    }

    public override void RunThread() {
      try {
        mapGenerator.BuildWalls(partition);
        barrier.SignalAndWait();
      }
      catch (Exception e) {
        // TODO Auto-generated catch block
        UnityEngine.Debug.Log("BARRIER EXCEPTION MESSAGE: " + e.Message + "\nBARRIER EXCEPTION: " + e.StackTrace);
      }
    }
  }
}