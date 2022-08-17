using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace StockQuoteAlert
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bem-Vindo ao monitoramento de ativos Stock Quote Alert");
            Console.WriteLine("------------------------------------------------------");

            // Primeira interação com o usuário, pedir os dados e armazená-los
            Console.WriteLine("Digite o código do ativo a ser monitorado, o preço de referência para venda e o preço de referência para compra");

            string parameters = Console.ReadLine();

            string[] parameters_list = parameters.Split(" ");

            string ativ = parameters_list[0];
            double min = double.Parse(parameters_list[1], CultureInfo.InvariantCulture);
            double max = double.Parse(parameters_list[2], CultureInfo.InvariantCulture);
        }
    }
}