using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel.Kernel;

namespace Model_Lab
{
    public partial class SmoModel : Model
    {
        // Класс события: срабатывание такта процессора
        public class K1 : TimeModelEvent<SmoModel>
        {
            //#region Атрибуты события
            //public Bid ZP;
            //#endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                while (Model.NCP < Model.maxNCP)
                {
                    Model.TTN++;
                    Model.VQ[0].Z.CNT++;
                    if (Model.VQ[0].Z.CNT == Model.VQ[0].Z.RNT)
                    {
                        Model.VQ.RemoveAt(0);
                        Model.NCP++;
                        Model.Tracer.AnyTrace("Процесс №" + Model.NCP + " снят с исполнения");
                    }
                }

                //var ev = new K1();                                
                //Model.PlanEvent(ev, 0.1);                          
                //Model.Tracer.PlanEventTrace(ev);
                //Model.Tracer.AnyTrace("NCP = " + Model.NCP + " maxNCP = " + Model.maxNCP);
            }
        }

        // Класс события: Создание нового процесса
        public class ProcessCreation : TimeModelEvent<SmoModel>
        {
            //#region Атрибуты события
            //public Bid ZP;
            //#endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                Model.Tracer.EventTrace(this);

                Random rnd = new Random();
                while (Model.currN < Model.maxN)
                {
                    Model.currN++;
                    Process process = new Process(Model.currN, rnd.Next(Model.minNT, Model.maxNT), 0);
                    var rec = new QRec(process);
                    Model.VQ.Add(rec);
                    Model.Tracer.AnyTrace("Добавлен процесс №" + Model.currN + "\tТакты: " + process.RNT);
                }
            }
        }
    }
}
