using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;

namespace UF2Practica1
{
	class MainClass
	{
		//Valors constants
		const String CLIENTS_FILE = "clients.csv";

		/* Cua concurrent
		 	Dos mètodes bàsics: 
		 		Cua.Enqueue per afegir a la cua
		 		bool success = Cua.TryDequeue(out clientActual) per extreure de la cua i posar a clientActual
		*/

		public static ConcurrentQueue<Client> cua = new ConcurrentQueue<Client>();

		public static void Main(string[] args)
		{

			var clock = new Stopwatch();
			var tasks = new List<Task>();
            //Recordeu-vos indicar la ruta del fitxer
            string filePath = System.IO.Path.GetFullPath(CLIENTS_FILE);
            

			try
			{
                //LLEGIM ARXIU CLIENTS
				using (StreamReader sr = new StreamReader(filePath))
				{
    					sr.ReadLine();
    					while (sr.Peek() != -1)
    					{
       						string line = sr.ReadLine();
        					var values = line.Split(',');
                                    //CREEM ELS CLIENTS AMB LES DADES DE L'ARXIU
                    				var tmp = new Client() { nom = values[0], carretCompra = Int32.Parse(values[1]) };
                    				cua.Enqueue(tmp);

       
    					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Error accedint a l'arxiu");
				Console.ReadKey();
				Environment.Exit(0);
			}


			clock.Start();


            // Instanciar les caixeres i afegir el task creat a la llista de tasks
            Caixera caix1 = new Caixera() { idCaixera = 1 };
            Caixera caix2 = new Caixera() { idCaixera = 2 };
            Caixera caix3 = new Caixera() { idCaixera = 3 };

            
            //Instanciem les TASK per cada CAIXERA
            var task1 = new Task(() =>
            {
                caix1.ProcessarCua(cua);     
            });

            var task2 = new Task(() =>
            {
                caix2.ProcessarCua(cua);
            });

            var task3 = new Task(() =>
            {
                caix3.ProcessarCua(cua);
            });

            //Iniciem les TASK
            task1.Start();
            task2.Start();
            task3.Start();

            //Apilem les TASK per a fer el WAIT
            tasks.Add(task1);
            tasks.Add(task2);
            tasks.Add(task3);

            // Procediment per esperar que acabin tots els threads abans d'acabar
            Task.WaitAll(tasks.ToArray());



			// Parem el rellotge i mostrem el temps que triga
			clock.Stop();
			double temps = clock.ElapsedMilliseconds / 1000;
			//Console.Clear();
			Console.WriteLine("Temps total Task: " + temps + " segons");
			Console.ReadKey();
		}
	}


    //CLASS CAIXERA
	#region ClassCaixera
	public class Caixera
	{
        //GETTERS/SETTERS
        public int idCaixera
		{
			get;
			set;
		}

        //MÈTODES
		public void ProcessarCua(ConcurrentQueue<Client> cua)
		{
            // Llegirem la cua extreient l'element
            // cridem al mètode ProcesarCompra passant-li el client
            Client clientActual;
            while (cua.TryDequeue(out clientActual))
            {
                ProcesarCompra(clientActual);
            }
   
        }


		private void ProcesarCompra(Client client)
		{

			Console.WriteLine("La caixera " + this.idCaixera + " comença amb el client " + client.nom + " que té " + client.carretCompra + " productes");

			for (int i = 0; i < client.carretCompra; i++)
			{
				this.ProcessaProducte();

			}

			Console.WriteLine(">>>>>> La caixera " + this.idCaixera + " ha acabat amb el client " + client.nom);
		}


		private void ProcessaProducte()
		{
			Thread.Sleep(TimeSpan.FromSeconds(1));
		}
	}

	#endregion


    //CLASS CLIENT
	#region ClassClient

	public class Client
	{
        //GETTERS/SETTERS
		public string nom
		{
			get;
			set;
		}


		public int carretCompra
		{
			get;
			set;
		}


	}

	#endregion
}
