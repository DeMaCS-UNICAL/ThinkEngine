using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbASP4Unity.it.unical.mat.embasp.languages.asp {
  public class SymbolicConstant {
    private string value;

    public string Value { get => value; set => this.value = value; }

    public SymbolicConstant(string value) {
      this.Value = value;
    }

    public SymbolicConstant() { }

    public override string ToString() {
      return Value;
    }
  }
}
