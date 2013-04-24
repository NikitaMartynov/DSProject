using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSProject
{
    class DataStore
    {
        private Dictionary<int, List<int>> dataStore; //DictionaryOfNodes<ListOfNodeValues>

        public DataStore( )
        {
            this.dataStore = new Dictionary< int, List<int> >();
        }

        public bool addNewNode(int id) {
            if (dataStore.ContainsKey(id))
                return false;
            else {
                dataStore.Add(id, new List<int>());
                return true;
            }
        }
        public bool write(int id, int value) {
            List<int> listOfNodeVals;
            dataStore.TryGetValue(id, out listOfNodeVals);
            if (listOfNodeVals == null) {
                return false;
            }
            else{
                listOfNodeVals.Add(value);
                return true;
            }
        }
        public Dictionary< int, List<int>> getDataStore() {
            return dataStore;
        }

        public double getAverage() {
            double average = 0;
            double sum = 0;
            foreach (KeyValuePair<int, List<int>> item in dataStore) {
                double averageI = 0;
                for (int i = 0; i < item.Value.Count; i++) {
                    averageI += item.Value.ElementAt(i);
                }
                averageI /= item.Value.Count;
                sum += averageI;
            }
            average = sum / dataStore.Count;
            return average;
        }
/*
        private int getLowestCounter() {
            int lowestCounter = 0;
            if (dataStore.Count != 0) {
                lowestCounter = int.MaxValue;
            }

            foreach (KeyValuePair<int, List<int> > item in dataStore) {
                if (lowestCounter > item.Value.Count) {
                    lowestCounter = item.Value.Count;
                }
            }
            return lowestCounter;
        }
 * */


    }
}
