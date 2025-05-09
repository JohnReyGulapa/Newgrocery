using ProductManagerApp;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProductManagerApp
{
    public partial class Form2 : Form
    {
        private ListView listViewProducts;
        private Button btnAdd;
        private List<Product> products;

        public Form2()
        {
            InitializeComponent();
            AttachEvents();
            LoadProductsAsync();
        }

        private void InitializeComponent()
        {
            this.Text = "Product Manager (Form2)";
            this.Size = new System.Drawing.Size(600, 400);

            listViewProducts = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill
            };

            listViewProducts.Columns.Add("ID", 50);
            listViewProducts.Columns.Add("Name", 200);
            listViewProducts.Columns.Add("Price", 100);
            listViewProducts.Columns.Add("Actions", 100);

            btnAdd = new Button
            {
                Text = "Add New Product",
                Dock = DockStyle.Bottom
            };

            this.Controls.Add(listViewProducts);
            this.Controls.Add(btnAdd);
        }

        private void AttachEvents()
        {
            btnAdd.Click += BtnAdd_Click;

            listViewProducts.MouseClick += (sender, e) =>
            {
                ListViewHitTestInfo info = listViewProducts.HitTest(e.X, e.Y);
                if (info.Item != null && info.SubItem == info.Item.SubItems[3])
                {
                    if (info.Item.Tag is Product selectedProduct)
                    {
                        // Remove the selected product
                        RemoveProduct(selectedProduct);
                    }
                }
            };
        }

        private async void LoadProductsAsync()
        {
            try
            {
                products = await ProductManager.ReadJsonFromFileAsync();
                DisplayProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayProducts()
        {
            listViewProducts.Items.Clear();

            foreach (var product in products)
            {
                ListViewItem item = new ListViewItem(product.Id.ToString());
                item.SubItems.Add(product.Name);
                item.SubItems.Add(product.Price.ToString("C"));
                item.SubItems.Add("Remove");
                item.Tag = product;
                listViewProducts.Items.Add(item);
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            if (products.Count >= 5)
            {
                MessageBox.Show("You can only add up to 5 products.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Form addForm = new Form())
            {
                addForm.Text = "Add New Product";
                addForm.Size = new System.Drawing.Size(300, 220);
                addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                addForm.StartPosition = FormStartPosition.CenterParent;
                addForm.MaximizeBox = false;
                addForm.MinimizeBox = false;

                var lblId = new Label { Text = "ID:", Left = 20, Top = 20, Width = 50 };
                var txtId = new TextBox { Left = 80, Top = 20, Width = 180 };

                var lblName = new Label { Text = "Name:", Left = 20, Top = 55, Width = 50 };
                var txtName = new TextBox { Left = 80, Top = 55, Width = 180 };

                var lblPrice = new Label { Text = "Price:", Left = 20, Top = 90, Width = 50 };
                var txtPrice = new TextBox { Left = 80, Top = 90, Width = 180 };

                var btnSave = new Button { Text = "Save", Left = 100, Top = 130, Width = 80 };

                btnSave.Click += async (s, args) =>
                {
                    if (int.TryParse(txtId.Text, out int id) &&
                        !string.IsNullOrWhiteSpace(txtName.Text) &&
                        double.TryParse(txtPrice.Text, out double price))
                    {
                        if (products.Exists(p => p.Id == id))
                        {
                            MessageBox.Show("A product with this ID already exists.",
                                "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        Product newProduct = new Product
                        {
                            Id = id,
                            Name = txtName.Text,
                            Price = price
                        };

                        products.Add(newProduct);
                        await ProductManager.WriteJsonToFileAsync(products);
                        DisplayProducts();
                        addForm.Close();
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid product information.",
                            "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };

                addForm.Controls.AddRange(new Control[] { lblId, txtId, lblName, txtName, lblPrice, txtPrice, btnSave });
                addForm.ShowDialog(this);
            }
        }

        private async void RemoveProduct(Product selectedProduct)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to remove this product?",
                                                 "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                products.Remove(selectedProduct);
                await ProductManager.WriteJsonToFileAsync(products);
                DisplayProducts();
            }
        }
    }
}
