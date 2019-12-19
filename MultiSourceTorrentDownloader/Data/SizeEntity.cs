using MultiSourceTorrentDownloader.Constants;
using System;

namespace MultiSourceTorrentDownloader.Data
{
    public class SizeEntity : IComparable
    {
        public double Value { get; set; }
        public string Postfix { get; set; }

        public override string ToString()
        {
            return $"{string.Format("{0:0.##}", Value)} {Postfix}";
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var that = obj as SizeEntity;
            if(that != null)
            {
                if ((this.Postfix == SizePostfix.KiloBytes && that.Postfix == SizePostfix.KiloBytes)
               || (this.Postfix == SizePostfix.MegaBytes && that.Postfix == SizePostfix.MegaBytes)
               || (this.Postfix == SizePostfix.GigaBytes && that.Postfix == SizePostfix.GigaBytes))
                {
                    return this.Value.CompareTo(that.Value);
                }

                if (this.Postfix == SizePostfix.KiloBytes
                    || (this.Postfix == SizePostfix.MegaBytes && that.Postfix == SizePostfix.GigaBytes))
                    return -1;//that is bigger

                return 1;//this is bigger when only combination is that.Postfix = KiloBytes
            }
            else
            {
                throw new ArgumentException("Object is not a SizeEntity");
            }
            
        }
    }
}
