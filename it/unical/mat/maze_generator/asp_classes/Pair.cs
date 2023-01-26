using System.Collections.Generic;

namespace it.unical.mat.asp_classes
{
	public class Pair<K, V>
	{

		private readonly K first;
		private readonly V second;

		public static Pair<K, V> CreatePair<k, v>(K first, V second)
		{
			return new Pair<K, V>(first, second);
		}

    public Pair(K first, V second)
		{
			this.first = first;
			this.second = second;
		}

		public virtual K First
		{
			get
			{
				return first;
			}
		}

		public virtual V Second
		{
			get
			{
				return second;
			}
		}


    public override bool Equals(object obj) {
      var pair = obj as Pair<K, V>;
      return pair != null &&
             EqualityComparer<K>.Default.Equals(first, pair.first) &&
             EqualityComparer<V>.Default.Equals(second, pair.second) &&
             EqualityComparer<K>.Default.Equals(First, pair.First) &&
             EqualityComparer<V>.Default.Equals(Second, pair.Second);
    }

    public override int GetHashCode() {
      var hashCode = 345486764;
      hashCode = hashCode * -1521134295 + EqualityComparer<K>.Default.GetHashCode(first);
      hashCode = hashCode * -1521134295 + EqualityComparer<V>.Default.GetHashCode(second);
      hashCode = hashCode * -1521134295 + EqualityComparer<K>.Default.GetHashCode(First);
      hashCode = hashCode * -1521134295 + EqualityComparer<V>.Default.GetHashCode(Second);
      return hashCode;
    }

  }
}