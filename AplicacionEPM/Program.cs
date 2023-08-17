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
            Console.WriteLine($"La cedula del cliente: {this.NombreCliente} es {this.ClienteCedulaCiudadania}, con estrato {this.ClienteEstrato} y su consumo mensual es {this.ClienteConsumoMes}");
        }    

    }

    public class Epm
    {
        public float PrecioAnteriorKilovatioHora { get; set; }
        public float PrecioActualKilovatioHora { get; set; }
        public float MetaAhorro { get; set; }
        public Dictionary<int, float> FactorEstrato { get; set; } = new Dictionary<int, float>();
        public List<Cliente> ListaClientes { get; set; } = new List<Cliente>();


        public Epm(float precioAnteriorKilovatioHora, float precioActualKilovatioHora)
        {

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


        public bool VerificarExistenciaCliente(int cedulaCliente)
        {
            foreach (Cliente verificarCliente in ListaClientes)
            {
                if (verificarCliente.ClienteCedulaCiudadania == cedulaCliente)
                {
                    return true;
                }
            }

            return false; // Si no se encuentra ninguna coincidencia en toda la lista, se retorna false.
        }

        public float CalcularMetaAhorro(Cliente cliente)
        {

            MetaAhorro = (cliente.ClienteConsumoMes + (cliente.ClienteConsumoMes * FactorEstrato[cliente.ClienteEstrato]));
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
            foreach (var buscarCliente in ListaClientes)
            {
                sumatoriaConsumo += buscarCliente.ClienteConsumoMes;
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

                Console.WriteLine($"Estrato {estrato}: Porcentaje de ahorro = {totalAhorro * 0.1 / CalcularTotalDescuentos():0.2}%");
            }
        }

        public int ContabilizarCobrosAdicionales(Cliente clienteComparar)
        {
            int clientesConCobroAdicional = 0;

            foreach (var buscarCliente in ListaClientes)
            {
                float consumoActual = buscarCliente.ClienteConsumoMes;
                float metaAhorro = CalcularMetaAhorro(clienteComparar);

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
        public Epm epm;

        public Menu(Epm epm)
        {
            this.epm = epm;
        }

        static void Main(string[] args)

        {
            Console.WriteLine("------------------------------------------------------------------------ ");
            Console.WriteLine("BIENVENIDO AL SISTEMA INCENTIVO AL AHORRO EN EL CONSUMO DE ENERGÍA DE EPM");
            Console.WriteLine("------------------------------------------------------------------------ ");

            Console.WriteLine("Ingrese el precio actual de kilovaltio hora: ");
            float kilovaltioActual = float.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese el precio anterior de kilovaltio hora: ");
            float kilovaltioAnterior = float.Parse(Console.ReadLine());

            Epm epm = new Epm(kilovaltioAnterior, kilovaltioActual);

        
            while (true)
            {

                Console.WriteLine("------------------------------------------------------------------------ ");

                Console.WriteLine("Seleccione una opcion: ");
                Console.WriteLine("1. Agregar un cliente");
                Console.WriteLine("2. Verificar existencia del cliente");
                Console.WriteLine("3. Valor a Pagar por el servicio de energía");
                Console.WriteLine("4. Promedio del consumo actual de energía");
                Console.WriteLine("5. Valor total por concepto de descuentos");
                Console.WriteLine("6. Porcentajes de ahorro por estrato");
                Console.WriteLine("7. Numero de clientes que tuvieron cobro adicional y superaron su meta");

                Console.WriteLine("------------------------------------------------------------------------ ");


                int opcion = int.Parse(Console.ReadLine());


                switch(opcion)

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
                        clienteNuevo.ImprimirDatosCliente();
                        float metaCliente = epm.CalcularMetaAhorro(clienteNuevo);
                        Console.WriteLine($"Meta de Ahorro del cliente {metaCliente}");
                        float parcialCliente = epm.CalcularPrecioParcial(clienteNuevo);
                        Console.WriteLine($"Precio Parcial {parcialCliente}");
                        float incentivoCliente = epm.CalcularPrecioIncentivo(clienteNuevo);
                        Console.WriteLine($"Precio Incentivo {incentivoCliente}"); 
                                                                 
                        break;

                    case 2:
                        Console.WriteLine("Por favor ingrese la cedula del cliente para verficar que si existan sus datos.: ");
                        int verificarCedula = int.Parse(Console.ReadLine());
                        epm.VerificarExistenciaCliente(verificarCedula);
                                           
                        break;

                    case 3:
                        Console.WriteLine($"El valor a pagar por el servicio de energía es: {epm.CalcularValorPagar}");

                        break;

                    case 4:
                        Console.WriteLine($"El promedio del consumo actual de energía es: {epm.CalcularPromedioConsumo}");

                        break;

                    case 5:
                        Console.WriteLine($" El valor total por concepto de descuentos a causa del incentivo es: {epm.CalcularTotalDescuentos}");

                        break;

                    case 6:
                        Console.WriteLine($"Porcentajes de ahorro por estrato {epm.MostrarPorcentajesAhorroPorEstrato}");

                        break;

                    case 7:
                        Console.WriteLine($"Numero total de clientes que tuvieron un cobro adicional: {epm.ContabilizarCobrosAdicionales}");

                        break;

                    default:

                        Console.WriteLine("Seleccione una opción valida");
                        break; 
                       
                }
            }
        }

    }
   
}
        
