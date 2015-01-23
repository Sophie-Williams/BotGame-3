using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    static class Direction
    {
        public static int[] convert(string dirStr)
        {
            bool flagX, flagY, flagZ;
            int[] outArray = new int[] { 0, 0, 0 };

            flagX = flagY = flagZ = false;
            foreach (char letter in dirStr)
            {
                if (char.ToUpper(letter) == 'N' && !flagX)
                {
                    flagX = true;
                    outArray[0] = -1;
                }
                if (char.ToUpper(letter) == 'S' && !flagX)
                {
                    flagX = true;
                    outArray[0] = 1;
                }
                if (char.ToUpper(letter) == 'W' && !flagY)
                {
                    flagY = true;
                    outArray[1] = -1;
                }
                if (char.ToUpper(letter) == 'E' && !flagY)
                {
                    flagY = true;
                    outArray[1] = 1;
                }
                if (char.ToUpper(letter) == 'U' && !flagZ)
                {
                    flagZ = true;
                    outArray[2] = -1;
                }
                if (char.ToUpper(letter) == 'D' && !flagZ)
                {
                    flagZ = true;
                    outArray[2] = 1;
                }
            }
            return outArray;
        }
    }
}
