using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Timers;
using System.IO.Ports;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Diagnostics;

namespace SonarLuna
{

    public partial class frmMain : Form
    {

        SerialPort portaSerial;


        Class1 entidade1 = new();

        public System.Windows.Forms.Timer temporizador = new System.Windows.Forms.Timer();

        int largura = 360, altura = 360, guia = 180;

        int graus;
        int centerX, centerY;
        int x, y;
        int distanciaMax = 300;

        int tx, ty, lim = 20;

        Bitmap bmp;
        Pen p, pL, ponteiro;
        Graphics g, entidade;
        List<string> lista = new();


        public frmMain()
        {
            InitializeComponent();
            portaSerial = new SerialPort();
        }


        private void UpdateRichTextBox(string pTexto)
        {
            if (richTextBoxDadosRecebidos.InvokeRequired)
                 richTextBoxDadosRecebidos.Invoke((MethodInvoker)(() => UpdateRichTextBox(pTexto)));         
            else
                richTextBoxDadosRecebidos.Text = pTexto;
        }

        public void eventoDadosRecebidosSerial(object sender, SerialDataReceivedEventArgs e)
        {
            if (portaSerial.IsOpen)
            {
                string dadosRecebidos = portaSerial.ReadLine();

                string[] valoresLidosDaSerial = dadosRecebidos.Split(";");

                UpdateRichTextBox(dadosRecebidos);

                entidade1.Distancia = Convert.ToDouble(valoresLidosDaSerial[0]);
                entidade1.Angulo = Convert.ToDouble(valoresLidosDaSerial[1]);
            }

        }

     
        public void FechaSerial()
        {
            if (portaSerial.IsOpen)
            {
                portaSerial.DataReceived -= eventoDadosRecebidosSerial;
                portaSerial.Close();
            }
        }
        private void buttonConectar_Click(object sender, EventArgs e)
        {
            string PortaEscolhida = cmbPortasArduino.Text;

            if (PortaEscolhida  != "")
            {
                if (!portaSerial.IsOpen)
                {
                    portaSerial = new SerialPort(PortaEscolhida, 9600);
                    portaSerial.Open();
                    portaSerial.DataReceived += eventoDadosRecebidosSerial;
                }
                else
                    MessageBox.Show("Serial já conectada!!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Escolha uma porta COM", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);





        }

       
        private void comboBoxComs_DropDown(object sender, EventArgs e)
        {
            string[] coms = SerialPort.GetPortNames();
            if (coms.Count() > 0)
            {
                string conteudoAntigo = cmbPortasArduino.Text;
                cmbPortasArduino.Items.Clear();
                cmbPortasArduino.Items.AddRange(coms);

                if (cmbPortasArduino.Items.Contains(conteudoAntigo))
                {
                    cmbPortasArduino.SelectedIndex = cmbPortasArduino.Items.IndexOf(conteudoAntigo);
                }
                else
                {
                    cmbPortasArduino.Text = "";
                    FechaSerial();
                }

            }
            else
            {
                cmbPortasArduino.Items.Clear();
                cmbPortasArduino.Text = "";
                FechaSerial();
            }
        }
        //ARDUINO


        private void frmMain_Load(object sender, EventArgs e)
        {

            bmp = new Bitmap(largura + 1, altura + 1);

            panel1.BackColor = Color.Black;

            centerX = largura / 2;
            centerY = altura / 2;

            graus = 0;

            temporizador.Interval = 30; //ms, demora aproximadamente 11/12 segundos pra dar uma volta
            temporizador.Tick += new EventHandler(timer_Elapsed);
            temporizador.Start();

        }
        private void Entidade(int distancia, double angulo)
        {

            double radianos = Math.PI / 180f * angulo;
            int distanciaPx = (int)(Math.Min(Math.Max(distancia, 0), distanciaMax) / (double)distanciaMax * (double)largura / 2d);

            int x = (int)(Math.Cos(radianos) * distanciaPx);
            int y = (int)(Math.Sin(radianos) * distanciaPx);

            entidade = Graphics.FromImage(bmp);
            pL = new Pen(Color.Red, 1f);
            entidade.DrawEllipse(pL, x - 5 + centerX, y - 5 + centerY, 10, 10);

        }
        private void timer_Elapsed(object sender, EventArgs e)
        {

            //atualiza radar
            graus++;
            if (graus == 360)
            {
                graus = 0;
                //Chamo a entidade q foi encontrada pelo radar
            }
            p = new Pen(Color.LimeGreen, 1f);

            ponteiro = new Pen(Color.LimeGreen, 2f);

            g = Graphics.FromImage(bmp);

            int vrau = (graus - lim) % 360;

            if (graus >= 0 && graus <= 180)
            {
                x = centerX + (int)(guia * Math.Sin(Math.PI * graus / 180));
                y = centerY - (int)(guia * Math.Cos(Math.PI * graus / 180));
            }
            else
            {
                x = centerX - (int)(guia * -Math.Sin(Math.PI * graus / 180));
                y = centerY - (int)(guia * Math.Cos(Math.PI * graus / 180));
            }
            if (vrau >= 0 && vrau <= 180)
            {
                tx = centerX + (int)(guia * Math.Sin(Math.PI * vrau / 180));
                ty = centerY - (int)(guia * Math.Cos(Math.PI * vrau / 180));
            }
            else
            {
                tx = centerX - (int)(guia * -Math.Sin(Math.PI * vrau / 180));
                ty = centerY - (int)(guia * Math.Cos(Math.PI * vrau / 180));
            }

            //circulos do sonar
            g.DrawEllipse(p, 0, 0, largura, altura);
            g.DrawEllipse(p, 90, 90, largura - 180, altura - 180);
            g.DrawEllipse(p, 45, 45, largura - 90, altura - 90); 
            g.DrawEllipse(p, 135, 135, largura - 270, altura - 270);

            g.DrawLine(p, new Point(centerX, 0), new Point(centerX, altura));  //Linha Vertical do Sonar
            g.DrawLine(p, new Point(0, centerY), new Point(largura, centerY)); //Linha Horizontal do Sonar


            //Ponteiro

            g.DrawLine(new Pen(Color.Black, 4f), new Point(centerX, centerY), new Point(tx, ty));

            g.DrawLine(ponteiro, new Point(centerX, centerY), new Point(x, y));

            Entidade((int)entidade1.Distancia, entidade1.Angulo);

            //carregar a picturebox
            pictureBox1.Image = bmp;
            g.Dispose();
            p.Dispose();



        }

    }
}