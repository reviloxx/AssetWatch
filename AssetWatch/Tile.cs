using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public abstract class Tile
    {
        private int windowStyle;
        private int hasUniqueWindowStyle;
        private int position;

        public string FiatCurrency
        {
            get => default(string);
            set
            {
            }
        }

        public int CurrentWorth
        {
            get => default(int);
            set
            {
            }
        }

        public int InvestedSum
        {
            get => default(int);
            set
            {
            }
        }

        public int HoldingsCount
        {
            get => default(int);
            set
            {
            }
        }

        public void SetWindowStyle()
        {
            throw new System.NotImplementedException();
        }
    }
}