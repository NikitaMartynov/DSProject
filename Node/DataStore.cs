using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSProject
{
    class DataStore
    {
        private Dictionary<int, List<int>> m_dataStore; //DictionaryOfNodes<ListOfNodeValues>

        public DataStore( )
        {
            m_dataStore = new Dictionary< int, List<int> >();
        }

        public bool write(int id, int value) {
            int lowestCounter = getLowestCounter();
            List<int> listOfNodeVals;
            m_dataStore.TryGetValue(id, out listOfNodeVals);
            if (listOfNodeVals == null) {
                m_dataStore.Add(id, new List<int>());
                m_dataStore.TryGetValue(id, out listOfNodeVals);
            }
            if (lowestCounter >= listOfNodeVals.Count) {
                listOfNodeVals.Add(value);
                return true;
            }
            else {
                listOfNodeVals.RemoveAt(listOfNodeVals.Count - 1);
                listOfNodeVals.Add(value);
            }
            return false;
        }

        public Dictionary< int, List<int>> getDataStore() {
            return m_dataStore;
        }

        public List<int[]> getLastTempfromAllNodes() {
            List<int[]> tempList = new List<int[]>();
            foreach (KeyValuePair<int, List<int>> item in m_dataStore) {
                int[] temp = new int[2];
                temp[0] = item.Key;
                temp[1] = item.Value.ElementAt(item.Value.Count - 1);
                tempList.Add(temp);
            }
            return tempList;
        }



        //not checked
        public double getAverage() {
            int lowestCounter = getLowestCounter();
            double average = 0;
            double sum = 0;
            foreach (KeyValuePair<int, List<int>> item in m_dataStore) {
                double averageI = 0;
                for (int i = 0; i < lowestCounter; i++) {
                    averageI += item.Value.ElementAt(i);
                }
                averageI /= lowestCounter;
                sum += averageI;

            }
            average = sum / m_dataStore.Count;
            return average;
        }

        private int getLowestCounter() {
            int lowestCounter = 0;
            if (m_dataStore.Count != 0) {
                lowestCounter = int.MaxValue;
            }

            foreach (KeyValuePair<int, List<int> > item in m_dataStore) {
                if (lowestCounter > item.Value.Count) {
                    lowestCounter = item.Value.Count;
                }
            }
            return lowestCounter;
        }


    }
}
