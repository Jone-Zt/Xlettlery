using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealManagement
{
    public class XLetteryAlgorithm
    {
        private IList<string[]> Ts;

        public XLetteryAlgorithm(IList<string[]> list=null)
        {
            if (list == null)
                Ts = new List<string[]>();
            else
                this.Ts = list;
        }
        public bool Add(string[] vs)
        {
            Ts.Add(vs);
            return true;
        }
        public IList<string> GetModelsWithType(PublicDefined.GameType type)
        {
            switch (type)
            {
                case PublicDefined.GameType.Singler:
                    return GetVsWithOne();
                case PublicDefined.GameType.Two:
                   return GetVsWithTwo();
                case PublicDefined.GameType.Three:
                    return GetVsWithThree();
                case PublicDefined.GameType.Fore:
                    return GetVsWithFore();
                case PublicDefined.GameType.Five:
                    return GetVsWithFive();
                case PublicDefined.GameType.Six:
                    return GetVsWithSix();
                case PublicDefined.GameType.Seven:
                    return GetVsWithSeven();
                case PublicDefined.GameType.Eight:
                    return GetVsWithEight();
                default:
                    throw new Exception("类型错误!");
            }
        }
        public IList<string> GetVsWithOne()
        {
            return Ts[0];
        }
        private IList<string> GetVsWithTwo()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Ts.Count - 1; i++)
            {
                for (int j = i + 1; j < Ts.Count; j++)
                {
                    List<string[]> al = new List<string[]>() { Ts[i], Ts[j] };
                    WorkOutUntiles outUntiles = new WorkOutUntiles();
                    outUntiles.Descartes(al, 0, list, string.Empty);
                }
            }
            return list;
        }
        private IList<string> GetVsWithThree()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Ts.Count - 1; i++)
            {
                for (int j = i + 1; j < Ts.Count; j++)
                {
                    for (int k = j + 1; k < Ts.Count; k++)
                    {
                        List<string[]> al = new List<string[]>() { Ts[i], Ts[j], Ts[k] };
                        WorkOutUntiles outUntiles = new WorkOutUntiles();
                        outUntiles.Descartes(al, 0, list, string.Empty);
                    }
                }
            }
            return list;
        }
        private IList<string> GetVsWithFore()
        {

            List<string> list = new List<string>();
            for (int i = 0; i < Ts.Count - 1; i++)
            {
                for (int j = i + 1; j < Ts.Count; j++)
                {
                    for (int k = j + 1; k < Ts.Count; k++)
                    {
                        for (int l = k+1; l < Ts.Count; l++)
                        {
                            List<string[]> al = new List<string[]>() { Ts[i], Ts[j], Ts[k],Ts[l]};
                            WorkOutUntiles outUntiles = new WorkOutUntiles();
                            outUntiles.Descartes(al, 0, list, string.Empty);
                        }
                    }
                }
            }
            return list;
        }
        private IList<string> GetVsWithFive()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Ts.Count - 1; i++)
            {
                for (int j = i + 1; j < Ts.Count; j++)
                {
                    for (int k = j + 1; k < Ts.Count; k++)
                    {
                        for (int l = k + 1; l < Ts.Count; l++)
                        {

                            for (int m = l+1; m < Ts.Count; m++)
                            {
                                List<string[]> al = new List<string[]>() { Ts[i], Ts[j], Ts[k], Ts[l],Ts[m]};
                                WorkOutUntiles outUntiles = new WorkOutUntiles();
                                outUntiles.Descartes(al, 0, list, string.Empty);
                            }
                        }
                    }
                }
            }
            return list;
        }
        private IList<string> GetVsWithSix()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Ts.Count - 1; i++)
            {
                for (int j = i + 1; j < Ts.Count; j++)
                {
                    for (int k = j + 1; k < Ts.Count; k++)
                    {
                        for (int l = k + 1; l < Ts.Count; l++)
                        {

                            for (int m = l + 1; m < Ts.Count; m++)
                            {
                                for (int n = m+1; n < Ts.Count; n++)
                                {
                                    List<string[]> al = new List<string[]>() { Ts[i], Ts[j], Ts[k], Ts[l], Ts[m],Ts[n]};
                                    WorkOutUntiles outUntiles = new WorkOutUntiles();
                                    outUntiles.Descartes(al, 0, list, string.Empty);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
        private IList<string> GetVsWithSeven()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Ts.Count - 1; i++)
            {
                for (int j = i + 1; j < Ts.Count; j++)
                {
                    for (int k = j + 1; k < Ts.Count; k++)
                    {
                        for (int l = k + 1; l < Ts.Count; l++)
                        {

                            for (int m = l + 1; m < Ts.Count; m++)
                            {
                                for (int n = m + 1; n < Ts.Count; n++)
                                {
                                    for (int s = n+1; s < Ts.Count; s++)
                                    {
                                        List<string[]> al = new List<string[]>() { Ts[i], Ts[j], Ts[k], Ts[l], Ts[m], Ts[n] ,Ts[s]};
                                        WorkOutUntiles outUntiles = new WorkOutUntiles();
                                        outUntiles.Descartes(al, 0, list, string.Empty);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
        private IList<string> GetVsWithEight()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Ts.Count - 1; i++)
            {
                for (int j = i + 1; j < Ts.Count; j++)
                {
                    for (int k = j + 1; k < Ts.Count; k++)
                    {
                        for (int l = k + 1; l < Ts.Count; l++)
                        {
                            for (int m = l + 1; m < Ts.Count; m++)
                            {
                                for (int n = m + 1; n < Ts.Count; n++)
                                {
                                    for (int s = n + 1; s < Ts.Count; s++)
                                    {
                                        for (int g = s+1; g < Ts.Count; g++)
                                        {
                                            List<string[]> al = new List<string[]>() { Ts[i], Ts[j], Ts[k], Ts[l], Ts[m], Ts[n], Ts[s],Ts[g] };
                                            WorkOutUntiles outUntiles = new WorkOutUntiles();
                                            outUntiles.Descartes(al, 0, list, string.Empty);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}
