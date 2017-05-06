using IMS.Model.Entity;
using IMS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Evaluate.Algorithms
{
    public static class Reliability
    {
        public static double MTBF(List<double> interval, out double alph, out double beta)
        {
            double MTBF =0; alph = 0; beta = 0;
            int n = interval.Count;
            if (n > 0)
            {
                double k = Math.Round(1 + 3.3 * Math.Log(n), 0);
                double tmax = Math.Round(interval.Max(), 2);
                double tmin = Math.Round(interval.Min(), 2);
                double deltaT = Math.Round((tmax - tmin) / k, 2);
                double[] up = new double[Convert.ToInt32(k)];
                double[] down = new double[Convert.ToInt32(k)];
                double[] ti = new double[Convert.ToInt32(k)];
                for (int i = 1; i < k; i++)
                {
                    up[0] = tmin;
                    up[i] = up[i - 1] + deltaT;
                    down[0] = tmin + deltaT;
                    down[i] = down[i - 1] + deltaT;
                }
                for (int i = 0; i < k; i++)
                {
                    ti[i] = (up[i] + down[i]) / 2;
                }
                int[] frequency = new int[Convert.ToInt32(k)];
                int[] Frequency = new int[Convert.ToInt32(k)];
                double[] fti = new double[Convert.ToInt32(k)];
                double[] Fti = new double[Convert.ToInt32(k)];
                for (int i = 0; i < k; i++)
                {
                    int count = 0;
                    for (int a = 0; a < n; a++)
                    {
                        if (up[i] <= interval[a] && interval[a] < down[i])
                            count++;
                    }
                    frequency[i] = count;
                    fti[i] = frequency[i] / (n * deltaT);
                }
                for (int i = 1; i < k; i++)
                {
                    Frequency[0] = frequency[0];
                    int a = new int();
                    for (int c = 0; c <= i; c++)
                    {
                        a += frequency[c];
                    }
                    Frequency[i] = a;
                    Fti[i] = Frequency[i] / (n * deltaT);
                }

                double[] xi = new double[n];
                double[] yi = new double[n];
                for (int i = 1; i <= n; i++)
                {
                    if (interval[i - 1] != 0)
                    {
                        xi[i - 1] = Math.Log(interval[i - 1]);
                        yi[i - 1] = Math.Log(Math.Log((n + 0.4) / (n + 0.7 - i)));
                    }
                }
                double Sigmax = new double();
                double Sigmay = new double();
                for (int i = 0; i <= n - 1; i++)
                {
                    Sigmax += xi[i];
                    Sigmay += yi[i];
                }
                double xbar = Sigmax / n;
                double ybar = Sigmay / n;
                double Sigmaxy = new double();
                double Sigmaxx = new double();
                double Sigmayy = new double();
                for (int i = 0; i <= n - 1; i++)
                {
                    Sigmaxy += xi[i] * yi[i];
                    Sigmaxx += xi[i] * xi[i];
                    Sigmayy += yi[i] * yi[i];
                }

                double B = (Sigmaxy - n * xbar * ybar) / (Sigmaxx - n * xbar * xbar);
                double A = ybar - B * xbar;
                double Alph = Math.Exp(-A / B);
                double Beta = B;

                double Rho = (Sigmaxy - n * xbar * ybar) / (Math.Pow((Sigmaxx - n * xbar * xbar) * (Sigmayy - n * ybar * ybar), 0.5));
                double RHO = 1.645 / Math.Pow((n - 1), 0.5);

                if (Rho > RHO)//线性相关
                {
                    double[] di = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        double[] bjs = new double[n];
                        bjs[i] = 1 - Math.Exp(-(Math.Pow((interval[i] / Alph), Beta)));
                        di[i] = Math.Abs(bjs[i] - ((i - 0.3) / (n + 0.4)));
                    }
                    Array.Sort(di);
                    double d = di[n - 1];
                    double D = 1.63 / (Math.Pow(n, 0.5));
                    if (D > d)//D检验
                    {
                        double T = 1 / Beta;
                        MTBF = Math.Round(Alph * Gamma(1 + (1 / Beta)), 0);
                        alph = Alph;
                        beta = Beta;
                    }
                }
            }
            return MTBF;
        }
        private static double Gamma(double x)
        {

            int i;
            double y, t, s, u;
            double[] a ={ 0.0000677106,-0.0003442342, 0.0015397681,
                                 -0.0024467480, 0.0109736958,-0.0002109075,
                                  0.0742379071, 0.0815782188, 0.4118402518,
                                  0.4227843370, 1.0};


            y = x;
            if (y <= 1.0)
            {
                t = 1.0 / (y * (y + 1.0));
                y = y + 2.0;
            }
            else
                if (y <= 2.0)
                {
                    t = 1.0 / y;
                    y = y + 1.0;
                }
                else
                    if (y <= 3.0)
                    {
                        t = 1.0;
                    }
                    else
                    {
                        t = 1.0;
                        while (y > 3.0)
                        {
                            y = y - 1.0;
                            t = t * y;
                        }
                    }
            s = a[0];
            u = y - 2.0;
            for (i = 1; i <= 10; i++)
            {
                s = s * u + a[i];
            }
            s = s * t;
            return (s);
        }

        public static DncRelated DncRelateReliability(MMDD_SBLY dncData) 
        {
            DncRelated dncRelatedReliability = new DncRelated();
            dncRelatedReliability.BootTime = dncData.RunTime + dncData.RunNull + dncData.RunDelay + dncData.PauseTime;
            dncRelatedReliability.OutWorkTime = dncData.RepairTime + dncData.DelayTime;
            dncRelatedReliability.SpindleRunningRate=dncData.RunTime_KB/(dncData.RunTime_KB+dncData.RunDelay+dncData.RunNull);
            dncRelatedReliability.BootRate = (dncData.RunTime + dncData.RunNull + dncData.RunDelay + dncData.PauseTime) / dncData.RunTime_KB;
            return dncRelatedReliability;
        }
     
    }
}