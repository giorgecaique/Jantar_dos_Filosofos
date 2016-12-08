using System;
using System.Threading;

//
// programa:  Jantar_dos_Filosofos
// programador: Giorge Caique
// data: 12/10/2016
// descricao: Programa que soluciona o problema do jantar dos filósofos usando threads e monitor
// entrada(s): Pelo sistema
// saida(s): Pela tela, o estado dos filósofos
//
namespace Jantar_dos_Filosofos
{
    class Program
    {
        static Philosopher philosopher1 = new Philosopher(1);
        static Philosopher philosopher2 = new Philosopher(2);
        static Philosopher philosopher3 = new Philosopher(3);
        static Philosopher philosopher4 = new Philosopher(4);
        static Philosopher philosopher5 = new Philosopher(5);

        static Fork fork1 = new Fork(1);
        static Fork fork2 = new Fork(2);
        static Fork fork3 = new Fork(3);
        static Fork fork4 = new Fork(4);
        static Fork fork5 = new Fork(5);

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("===================================== O JANTAR DOS FILÓSOFOS =====================================");

                Printer.Start();

                Thread tPhilosopher1 = new Thread(() => philosopher1.Eat(fork1, fork5));
                Thread tPhilosopher2 = new Thread(() => philosopher2.Eat(fork2, fork1));
                Thread tPhilosopher3 = new Thread(() => philosopher3.Eat(fork3, fork2));
                Thread tPhilosopher4 = new Thread(() => philosopher4.Eat(fork4, fork3));
                Thread tPhilosopher5 = new Thread(() => philosopher5.Eat(fork5, fork4));

                tPhilosopher1.Start();
                tPhilosopher2.Start();
                tPhilosopher3.Start();
                tPhilosopher4.Start();
                tPhilosopher5.Start();

            }
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }

            Console.ReadKey();
        }
    }


    public class Philosopher
    {
        public int Id { get; set; }
        public bool IsEating { get; set; }
        static object eatMonitor = new object();

        Random r = new Random();

        public Philosopher(int id)
        {
            this.Id = id;
        }

        #region Public Methods
        public void Eat(Fork forkA, Fork forkB)
        {
            this.IsEating = false;
            while (true)
            {
                Monitor.Enter(eatMonitor);
                Monitor.Pulse(eatMonitor);
                try
                {

                    if (forkA.InUse == false)
                    {
                        forkA.Use();
                        Thread.Sleep(r.Next(2000, 3500));


                        Printer.PickUpFork(this.Id, forkA.ForkId);
                        if (forkB.InUse == false)
                        {
                            this.IsEating = true;
                            forkB.Use();

                            Printer.PickUpFork(this.Id, forkB.ForkId);
                            Printer.Eating(this.Id);
                        }
                        else
                        {
                            Printer.ForkInUse(forkB.ForkId);
                            forkA.Drop();
                            Printer.DropFork(this.Id, forkA.ForkId);
                        }

                    }
                    else
                    {
                        Printer.ForkInUse(forkA.ForkId);
                        Printer.Thinking(this.Id);
                    }
                }

                finally
                {
                    Thread.Sleep(1500);

                    Monitor.Wait(eatMonitor);

                    if (this.IsEating == true)
                    {
                        forkA.Drop();
                        forkB.Drop();
                        Printer.DropFork(this.Id, forkA.ForkId, forkB.ForkId);
                        Printer.Thinking(this.Id);
                        this.IsEating = false;
                    }
                    Monitor.Exit(eatMonitor);
                }
            }
        }


        #endregion
    }

    public class Fork
    {
        public bool InUse { get; set; }
        public int ForkId { get; set; }

        public Fork(int id)
        {
            this.ForkId = id;
        }

        public void Use()
        {
            this.InUse = true;
        }

        public void Drop()
        {
            this.InUse = false;
        }
    }

    static public class Printer
    {
        static public void Start()
        {
            Console.WriteLine("{0} {1} {2} {3} {4}", "Filósofo 1        |", "Filósofo 2        |", "Filósofo 3        |", "Filósofo 4        |", "Filósofo 5        |");
            Console.WriteLine("==================================================================================================|");
        }

        static public void Thinking(int philosopherId)
        {
            switch (philosopherId)
            {
                case 1: Console.WriteLine("{0} {1} {2} {3} {4}", "pensando          |", "                  |", "                  |", "                  |", "                  |"); break;
                case 2: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "pensando          |", "                  |", "                  |", "                  |"); break;
                case 3: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "pensando          |", "                  |", "                  |"); break;
                case 4: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "pensando          |", "                  |"); break;
                case 5: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "                  |", "pensando          |"); break;
            }
        }

        static public void Eating(int philosopherId)
        {
            switch (philosopherId)
            {
                case 1: Console.WriteLine("{0} {1} {2} {3} {4}", "comendo           |", "                  |", "                  |", "                  |", "                  |"); break;
                case 2: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "comendo           |", "                  |", "                  |", "                  |"); break;
                case 3: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "comendo           |", "                  |", "                  |"); break;
                case 4: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "comendo           |", "                  |"); break;
                case 5: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "                  |", "comendo           |"); break;
            }
        }
        static public void PickUpFork(int philosopherId, int fork)
        {
            switch (philosopherId)
            {
                case 1: Console.WriteLine("{0} {1} {2} {3} {4}", "pegou garfo " + fork + "     |", "                  |", "                  |", "                  |", "                  |"); break;
                case 2: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "pegou garfo " + fork + "     |", "                  |", "                  |", "                  |"); break;
                case 3: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "pegou garfo " + fork + "     |", "                  |", "                  |"); break;
                case 4: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "pegou garfo " + fork + "     |", "                  |"); break;
                case 5: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "                  |", "pegou garfo " + fork + "     |"); break;
            }
        }

        static public void PickUpFork(int philosopherId, int forkA, int forkB)
        {
            switch (philosopherId)
            {
                case 1: Console.WriteLine("{0} {1} {2} {3} {4}", "pegou garfo " + forkA + " e " + forkB + " |", "                   |", "                   |", "                   |", "                   |"); break;
                case 2: Console.WriteLine("{0} {1} {2} {3} {4}", "                   |", "pegou garfo " + forkA + " e " + forkB + " |", "                   |", "                   |", "                   |"); break;
                case 3: Console.WriteLine("{0} {1} {2} {3} {4}", "                   |", "                   |", "pegou garfo " + forkA + " e " + forkB + " |", "                   |", "                   |"); break;
                case 4: Console.WriteLine("{0} {1} {2} {3} {4}", "                   |", "                   |", "                   |", "pegou garfo " + forkA + " e " + forkB + " |", "                   |"); break;
                case 5: Console.WriteLine("{0} {1} {2} {3} {4}", "                   |", "                   |", "                   |", "                   |", "pegou garfo " + forkA + " e " + forkB + " |"); break;
            }
        }

        static public void DropFork(int philosopherId, int fork)
        {
            switch (philosopherId)
            {
                case 1: Console.WriteLine("{0} {1} {2} {3} {4}", "soltou garfo " + fork + "    |", "                  |", "                  |", "                  |", "                  |"); break;
                case 2: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "soltou garfo " + fork + "    |", "                  |", "                  |", "                  |"); break;
                case 3: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "soltou garfo " + fork + "    |", "                  |", "                  |"); break;
                case 4: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "soltou garfo " + fork + "    |", "                  |"); break;
                case 5: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "                  |", "soltou garfo " + fork + "    |"); break;
            }
        }

        static public void DropFork(int philosopherId, int forkA, int forkB)
        {
            switch (philosopherId)
            {
                case 1: Console.WriteLine("{0} {1} {2} {3} {4}", "soltou garfo " + forkA + " e " + forkB + "|", "                  |", "                  |", "                  |", "                  |"); break;
                case 2: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "soltou garfo " + forkA + " e " + forkB + "|", "                  |", "                  |", "                  |"); break;
                case 3: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "soltou garfo " + forkA + " e " + forkB + "|", "                  |", "                  |"); break;
                case 4: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "soltou garfo " + forkA + " e " + forkB + "|", "                  |"); break;
                case 5: Console.WriteLine("{0} {1} {2} {3} {4}", "                  |", "                  |", "                  |", "                  |", "soltou garfo " + forkA + " e " + forkB + "|"); break;
            }
        }

        static public void ForkInUse(int fork)
        {
            Console.WriteLine("======================================= Garfo " + fork + " em uso ===========================================|");
        }

    }

}
