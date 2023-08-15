using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace EpmApp
{
    public class Cliente
    {
        public int ClienteCedulaCiudadania { get; set; }
        public string NombreCliente { get; set; }
        public float ClienteConsumoMes { get; set; }

        public int ClienteEstrato { get; set; }

        public Cliente(string nombreCliente, int clienteCedulaCiudadania, float clienteConsumoMes, int clienteEstrato)
        {
            this.NombreCliente = nombreCliente;
            this.ClienteCedulaCiudadania = clienteCedulaCiudadania;
            this.ClienteConsumoMes = clienteConsumoMes;
            this.ClienteEstrato = clienteEstrato;
        }

        public void ImprimirDatosCliente()
        {
            Console.WriteLine($"La cedula del cliente: {this.NombreCliente} es {this.ClienteCedulaCiudadania} de estrato {this.ClienteEstrato} y su consumo mensual es {this.ClienteConsumoMes}");
        }
    }

    public class Epm
    {
        public float PrecioAnteriorKilovatioHora { get; set; }
        public float PrecioActualKilovatioHora { get; set; }
        public float MetaAhorro { get; set; }
        public Dictionary<int, float> FactorEstrato { get; set; } = new Dictionary<int, float>();
        public List<Cliente> ListaClientes { get; set; } = new List<Cliente>();


        public Epm(float precioAnteriorKilovatioHora, float precioActualKilovatioHora, int factor_estrato)
        {
            // Inicializar los factores de estrato en el constructor
            FactorEstrato[1] = 0.5f;
            FactorEstrato[2] = 0.7f;
            FactorEstrato[3] = 0.8f;
            FactorEstrato[4] = 0.9f;
            FactorEstrato[5] = 1.0f;
            FactorEstrato[6] = 1.4f;

            this.PrecioActualKilovatioHora = precioActualKilovatioHora;
            this.PrecioAnteriorKilovatioHora = precioAnteriorKilovatioHora;
        }


        public void AgregarCliente(Cliente cliente)
        {
            ListaClientes.Add(cliente);
        }


        public float CalcularMetaAhorro(int factor_estrato, Cliente cliente)
        {
            MetaAhorro = (cliente.ClienteConsumoMes * (PrecioAnteriorKilovatioHora - PrecioActualKilovatioHora)) *
                         FactorEstrato[factor_estrato];

            return MetaAhorro;
        }

        public float CalcularPrecioParcial(Cliente cliente)
        {
            float precioParcial = cliente.ClienteConsumoMes * PrecioActualKilovatioHora;
            return precioParcial;
        }


        public float CalcularPrecioIncentivo(Cliente cliente)
        {
            float precioIncentivo = (MetaAhorro - cliente.ClienteConsumoMes) * PrecioActualKilovatioHora;
            return precioIncentivo;
        }


        public float CalcularValorPagar(Cliente cliente)
        {
            float precioParcial = CalcularPrecioParcial(cliente);
            float precioIncentivo = CalcularPrecioIncentivo(cliente);

            if (precioIncentivo < 0)
            {
                float precioTotal = precioParcial + precioIncentivo;
                return precioTotal;
            }
            else
            {
                float precioTotal = precioParcial - precioIncentivo;
                return precioTotal;
            }
        }



        public float CalcularPromedioConsumo()
        {
            float sumatoriaConsumo = 0;
            foreach (var cliente in ListaClientes)
            {
                sumatoriaConsumo += cliente.ClienteConsumoMes;
            }

            float promedioConsumo = sumatoriaConsumo / ListaClientes.Count;
            return promedioConsumo;
        }


        public float CalcularTotalDescuentos()
        {
            float totalDescuentos = 0;
            foreach (var cliente in ListaClientes)
            {
                totalDescuentos += CalcularPrecioIncentivo(cliente);
            }

            return totalDescuentos;
        }


        public void MostrarPorcentajesAhorroPorEstrato()
        {
            Dictionary<int, float> ahorroPorEstrato = new Dictionary<int, float>();

            foreach (var cliente in ListaClientes)
            {
                int estrato = cliente.ClienteEstrato;
                float ahorro = CalcularPrecioIncentivo(cliente);

                if (!ahorroPorEstrato.ContainsKey(estrato))
                {
                    ahorroPorEstrato[estrato] = 0;
                }

                ahorroPorEstrato[estrato] += ahorro;
            }

            foreach (var kvp in ahorroPorEstrato)
            {
                int estrato = kvp.Key;
                float totalAhorro = kvp.Value;

                Console.WriteLine($"Estrato {estrato}: Porcentaje de ahorro = {totalAhorro * 100 / CalcularTotalDescuentos():0.2}%");
            }
        }


        public int ContabilizarCobrosAdicionales()
        {
            int clientesConCobroAdicional = 0;

            foreach (var cliente in ListaClientes)
            {
                float consumoActual = cliente.ClienteConsumoMes;
                float metaAhorro = CalcularMetaAhorro(cliente.ClienteEstrato, cliente);

                if (consumoActual > metaAhorro)
                {
                    clientesConCobroAdicional++;
                }
            }

            return clientesConCobroAdicional;
        }
    }

    public class Menu
    {
        private Epm epm;

        public Menu( Epm epm )
        {
            this.epm = epm;

        }

        static void Main(string[] args)
        {
            Epm epm = new Epm(precioAnteriorKilovatioHora: 200f, precioActualKilovatioHora: 500f, 5);
           
            while (true)
            {
                Console.WriteLine("-------------------------------------------------------------------------");
                Console.WriteLine("BIENVENIDO AL SISTEMA INCENTIVO AL AHORRO EN EL CONSUMO DE ENERGÍA DE EPM");
                Console.WriteLine("-------------------------------------------------------------------------");
                Console.WriteLine("Seleccione una opcion: ");
                Console.WriteLine("1. Agregar un cliente");
                Console.WriteLine("2. Valor a Pagar por el servicio de energía");
                Console.WriteLine("3. Valor total por concepto de descuentos");
                Console.WriteLine("4. Mostrar Porcentajes de Ahorro por estrato");
                Console.WriteLine("-------------------------------------------------------------------------");

                int opcion = int.Parse(Console.ReadLine());

                switch( opcion )
                {
                    case 1:
                        Console.WriteLine("Ingrese los datos del ciente");

                        Console.WriteLine("Nombre del cliente: ");
                        string nombreCliente = Console.ReadLine();

                        Console.WriteLine("Cédula del cliente: ");
                        int clienteCedulaCiudadania = int.Parse(Console.ReadLine());

                        Console.WriteLine("Ingrese el consumo actual/mes del cliente: ");
                        float consumoMes = float.Parse(Console.ReadLine());

                        Console.WriteLine("Ingrese el Estrato del cliente: ");
                        int estratoCliente = int.Parse(Console.ReadLine());

                        Cliente clienteNuevo = new Cliente(nombreCliente, clienteCedulaCiudadania, consumoMes, estratoCliente);
                        epm.AgregarCliente(clienteNuevo);

                        break;

                    case 2:
                        Console.WriteLine("Ingrese cedula del cliente: ");
                        int buscarCedula = int.Parse(Console.ReadLine());

                        Cliente presentaCedula = null;

                        foreach (Cliente cliente in epm.ListaClientes)
                        {
                            if (cliente.ClienteCedulaCiudadania == buscarCedula)
                            {
                                presentaCedula = cliente;
                                break;
                            }

                        }

                        if (presentaCedula != null)
                        {
                            presentaCedula.ImprimirDatosCliente();
                            float pagoTotal = epm.CalcularValorPagar(presentaCedula);
                            Console.WriteLine($"El valor de servicios que debe pagar el cliente es:{pagoTotal}");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("El cliente no existe");
                        }
                        break;

                       
                    case 3:
                        Console.WriteLine($"El total de descuentos es: {epm.CalcularTotalDescuentos()}");
                        break;

                    case 4:
                        epm.MostrarPorcentajesAhorroPorEstrato();
                        break;

                    case 5:
                        Console.WriteLine($"La cantidad de clientes con cobros adicionales es: {epm.ContabilizarCobrosAdicionales}");
                        break;

                    default:
                        Console.WriteLine("Ingrese una opcion valida");
                        break;


                }
            }
           
 

        }
            
                
            
    }

        

        
            

}
        




