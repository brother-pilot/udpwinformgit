using IPAddressControlLib;
using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;


namespace udpwinformgit
{

	public class Form1 : Form, IView
	{
		//делаем общими данные для всех методов этого класса
		TableLayoutPanel table;
		TableLayoutPanel tableIn;
		Model mod;
		Label labelStatus;
		IPAddressControl maskedTextBoxIP;
		MaskedTextBox maskedTextBoxMAC;

		public string Status { get => this.labelStatus.Text; set => this.labelStatus.Text = value; }
		public string Mac { get => this.maskedTextBoxMAC.Text; set => this.maskedTextBoxMAC.Text = value; }
		public string Ip { get => this.maskedTextBoxIP.Text; set => this.maskedTextBoxIP.Text = value; }

		//А.2 Логика представления должна иметь ссылку на экземпляр презентера
		Presenter p;

		public Form1(Model mod)
		{
			//передаем форму в Presenter
			this.p = new Presenter(this, mod);
			//this.mod = mod;

			//this.mod = mod;
			//будем создавать две вложенные в друг друга таблицы
			table = new TableLayoutPanel();
			tableIn = new TableLayoutPanel();
			Label labelHeader = new Label//ну еще конструктор переделали на синтаксис сокращенной инициилизации
			{
				Location = new System.Drawing.Point(0, 0),
				Text = "Please, enter sourse IP and MAC Adress \n" +
				"For saving list adresses use button Add\n" +
				"For start test use button Start",
				Dock = DockStyle.Fill
			};
			labelStatus = new Label()
			{
				TabIndex = 2,
				Dock = DockStyle.Fill
			};

			table.RowStyles.Clear();
			table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
			table.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
			table.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
			table.Dock = DockStyle.Fill;
			Controls.Add(table);
			table.Controls.Add(labelHeader);
			table.Controls.Add(labelStatus, 0, 2);
			table.Controls.Add(tableIn);
			InitializeTableIn();

		}

		private void InitializeTableIn()
		{
			maskedTextBoxIP = new IPAddressControl
			{
				AllowInternalTab = false,
				AutoHeight = true,
				Dock = DockStyle.Fill,
				Name = "maskedTextBoxIP",
				TabIndex = 1,
				Text = "..."
			};
			// 
			// maskedTextBox for Mac 
			//
			maskedTextBoxMAC = new MaskedTextBox
			{
				Dock = DockStyle.Fill,
				Mask = ">AA-AA-AA-AA-AA-AA",
				Name = "maskedTextBoxMAC",
				TabIndex = 0,
				TextAlign = HorizontalAlignment.Center,
				ValidatingType = typeof(PhysicalAddress),
			};
			// 
			// buttonAdd
			// 
			Button buttonAdd = new Button
			{
				Name = "buttonAdd",
				Dock = DockStyle.Fill,
				TabIndex = 3,
				Text = "Add",
			};
			// 
			// buttonStart
			// 
			Button buttonStart = new Button
			{
				Name = "buttonStart",
				Dock = DockStyle.Fill,
				TabIndex = 3,
				Text = "Start",
			};

			tableIn.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
			tableIn.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
			tableIn.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			tableIn.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			tableIn.Controls.Add(maskedTextBoxIP, 0, 0);
			tableIn.Controls.Add(maskedTextBoxMAC, 1, 0);
			tableIn.Controls.Add(buttonAdd, 0, 1);
			tableIn.Controls.Add(buttonStart, 1, 1);
			tableIn.Dock = DockStyle.Fill;
			//events for controls
			maskedTextBoxIP.FieldChangedEvent += p.TextChangedIP;
			maskedTextBoxMAC.TextChanged += p.TextChangedMAC;
			buttonAdd.Click += p.ClearAndSave;
			buttonStart.Click += FinishIPAndMAC;
		}

		void FinishIPAndMAC(object sender, EventArgs e)
		{
			if (p.FinishIPAndMAC())
				this.Close();
		}

		private void Mod_ClearIpAndMac()
		{
			this.maskedTextBoxIP.Clear();
			this.maskedTextBoxMAC.Clear();
		}
	}




}

