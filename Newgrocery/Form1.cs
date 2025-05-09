using static ProductManagerApp.ProductManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductManagerApp
{
    public partial class Form1 : Form
    {
        private TextBox txtJsonContent;
        private Button btnRefresh;
        private Button btnOpenForm2;

        public Form1()
        {
            InitializeComponent();
            LoadJsonAsync();
        }

        private void InitializeComponent()
        {
            this.Text = "JSON Viewer (Form1)";
            this.Size = new System.Drawing.Size(600, 500);

            txtJsonContent = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Dock = DockStyle.Fill
            };

            btnRefresh = new Button
            {
                Text = "Refresh JSON",
                Dock = DockStyle.Bottom
            };
            btnRefresh.Click += BtnRefresh_Click;

            btnOpenForm2 = new Button
            {
                Text = "Open Product Manager",
                Dock = DockStyle.Bottom
            };
            btnOpenForm2.Click += BtnOpenForm2_Click;

            this.Controls.Add(txtJsonContent);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnOpenForm2);
        }

        private async Task LoadJsonAsync()
        {
            try
            {
                List<Product> existingProducts = new List<Product>();

                // If the file exists, load existing products
                if (File.Exists(ProductManager.FilePath))
                {
                    existingProducts = await ProductManager.ReadJsonFromFileAsync();
                }

                // If there are no products or fewer than 5, initialize up to 5
                if (existingProducts.Count == 0)
                {
                    var sampleProducts = new List<Product>
                    {
                        new Product { Id = 1, Name = "Laptop", Price = 800.00 },
                        new Product { Id = 2, Name = "Phone", Price = 500.00 },
                        new Product { Id = 3, Name = "Tablet", Price = 300.00 },
                        new Product { Id = 4, Name = "Monitor", Price = 250.00 },
                        new Product { Id = 5, Name = "Keyboard", Price = 50.00 }
                    };

                    await ProductManager.WriteJsonToFileAsync(sampleProducts);
                    existingProducts = sampleProducts;
                }
                else if (existingProducts.Count > 5)
                {
                    // Trim the list to only 5 items
                    existingProducts = existingProducts.GetRange(0, 5);
                    await ProductManager.WriteJsonToFileAsync(existingProducts);
                }

                string jsonContent = await File.ReadAllTextAsync(ProductManager.FilePath);
                txtJsonContent.Text = jsonContent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading JSON file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadJsonAsync();
        }

        private void BtnOpenForm2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }
    }
}
