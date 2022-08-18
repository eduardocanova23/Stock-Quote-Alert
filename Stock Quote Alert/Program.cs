using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Configuration;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StockQuoteAlert
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bem-Vindo(a) ao monitoramento de ativos Stock Quote Alert");
            Console.WriteLine("------------------------------------------------------");

            // Primeira interação com o usuário, pedir os dados e armazená-los
            Console.WriteLine("Digite o código do ativo a ser monitorado, o preço de referência para venda e o preço de referência para compra");

            string parameters = Console.ReadLine();
            string[] parameters_list = parameters.Split(" ");

            // Pegar possíveis erros de digitação
            try
            {
                string erroativ = parameters_list[0];
                double erromin = double.Parse(parameters_list[1], CultureInfo.InvariantCulture);
                double erromax = double.Parse(parameters_list[2], CultureInfo.InvariantCulture);
            }

            catch (Exception e)
            {
                Console.WriteLine("Você inseriu dados inválidos");
                Console.WriteLine("Error {0}", e.Message);
                Environment.Exit(0);
            }

            string ativ = parameters_list[0];
            double min = double.Parse(parameters_list[1], CultureInfo.InvariantCulture);
            double max = double.Parse(parameters_list[2], CultureInfo.InvariantCulture);

            // Avisar se os valores são inválidos e fechar o aplicativo
            if (min >= max)
            {
                Console.WriteLine("Você inseriu valores inválidos para o preço máximo e mínimo");
                Environment.Exit(0);
            }



            Console.WriteLine("Vou monitorar o ativo {0} e te mandar um email se a cotação desse ativo ficar abaixo de {1} reais," +
                " ou se ele ficar acima de {2} reais", ativ, min, max);

            Console.WriteLine("Por favor, insira o email que receberá os avisos");

            // Identificar se o email inserido é válido, e continuar pedindo um input caso não seja
            bool correto = false;
            string endEmail;

            do
            {
                endEmail = Console.ReadLine();
                if (IsValidEmail(endEmail))
                {
                    correto = true;
                }
                else
                {
                    Console.WriteLine("Esse email é inválido, digite novamente: ");
                }
            } while (correto is false);

             

            while (true)
            {
                JObject json = JObject.Parse(ApiHelper.Instance.CallWebAPI(ativ, min, max));

                // Extração do preço do objeto json que foi retornado pela API
                double price = Convert.ToDouble(json["results"][ativ.ToUpper()]["price"].ToString());

                // Comparar os valores dos preços e tomar a decisão de qual email enviar
                // Booleano usado para fazer com que a função que envia o email saiba construir o corpo do email de acordo
                // com o preço do ativo. Se o ativo está acima do limite, maior = true, e se o ativo está abaixo do limite
                // então maior = false. Vale reparar que a função de enviar o email nunca será chamada se o preço estiver dentro
                // da faixa

                if (price < min)
                {
                    SendWarnEmail(false, ativ, min, max, endEmail);
                    Console.WriteLine("Parece que seu ativo está abaixo do limite inferior, e agora está valendo R$"+price+". " +
                        "Cheque sua caixa de entrada no email informado");
                }
                if (price > max)
                {
                    SendWarnEmail(true, ativ, min, max, endEmail);
                    Console.WriteLine("Parece que seu ativo está acima do limite superior, e agora está valendo R$"+price+". " +
                        "Cheque sua caixa de entrada no email informado");
                }
                Thread.Sleep(180000);
            }

            Console.Write("Pressione qualquer tecla para fechar o app");
            Console.ReadKey();

            bool IsValidEmail(string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }

        }

        

        // Função de enviar email 
        static async Task SendWarnEmail(bool maior, string ativ, double min, double max, string endEmail)
        {
            using (MailMessage mail = new MailMessage())
            {
                // Criação do corpo do email
                mail.From = new MailAddress("eduardocanova23@outlook.com");
                mail.To.Add(endEmail);


                // Booleano que indica se o valor do ativo é maior do que o estipulado
                if (maior)
                {
                    mail.Subject = "Ativo ultrapassou o limite superior";
                    mail.Body = "<h1>Olá, venho informar que seu ativo " + ativ + " ultrapassou o limite superior estipulado por você de R$"+ max +" .</h1>";
                }
                else
                {
                    mail.Subject = "Ativo está abaixo do limite inferior";
                    mail.Body = "<h1>Olá, venho informar que seu ativo " + ativ + " está abaixo do limite inferior estipulado por você de R$"+ min +" .</h1>";
                }

                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com", 25))
                {
                    // O código lê o arquivo de configuração com as variáveis de email, senha, server escolhido e sua respectiva
                    // porta da camada de transporte

                    // Então o server SMTP é configurado e o email está pronto para ser enviado
                    string adress = ConfigurationManager.AppSettings.Get("EmailAdress");
                    string password = ConfigurationManager.AppSettings.Get("password");
                    string SmtpServer = ConfigurationManager.AppSettings.Get("server");
                    double port = Convert.ToDouble(ConfigurationManager.AppSettings.Get("port"));
                    smtp.Credentials = new System.Net.NetworkCredential(adress, password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

            }

        }

        // Fazer tratamento de erro de código de ativo inválido
        // Passar parâmetros por referência ao invés de passar várias variáveis em cada função
        // Fazer com que o email diga qual é o valor atual do ativo
        // Tratar erros de API e de conexão
        // Tratar erros de credencial e email inválido
        // Maneira melhor de configurar o server SMTP?
        // Organizar melhor e dividir tarefas nas funções da api e de envio de email

        [Serializable]
        private class IndexOutOfRange : Exception
        {
            public IndexOutOfRange()
            {
            }

            public IndexOutOfRange(string? message) : base(message)
            {
            }

            public IndexOutOfRange(string? message, Exception? innerException) : base(message, innerException)
            {
            }

            protected IndexOutOfRange(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}