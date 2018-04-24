using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;


namespace SuperAdventure
{
    public partial class TradingScreen : Form
    {
        private Player _currentPlayer;

        public TradingScreen(Player player)
        {

            _currentPlayer = player;
            InitializeComponent();

            //Style, to display numeric column values
            DataGridViewCellStyle rightAllignedCellStyle = new DataGridViewCellStyle();
            rightAllignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //Populate the datagrid for the player's inventory
            dgvMyItems.RowHeadersVisible = false;
            dgvMyItems.AutoGenerateColumns = false;

            //this hidden column hold the item id so we know which item to sell
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Description",
                Width = 100,
                HeaderText = "Name"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30,
                DefaultCellStyle = rightAllignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 35,
                DefaultCellStyle = rightAllignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvMyItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Sell 1",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });

            //Bind the Player's inventory to the dataGridView
            dgvMyItems.DataSource = _currentPlayer.Inventory;

            //When the user clicks on a row, call this function
            dgvMyItems.CellClick += dgvMyItems_CellClick;

            //Populate the datagrid for the vendor's inventory
            dgvVendorItems.RowHeadersVisible = false;
            dgvVendorItems.AutoGenerateColumns = false;

            //this hidden column hold the item ID so we know which item to sell
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "Description"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 35,
                DefaultCellStyle = rightAllignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvVendorItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Buy 1",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });

            //BInd the vendor's inventory to the datagridview
            dgvVendorItems.DataSource = _currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;

            //When the user clicks on a row call this function
            dgvVendorItems.CellClick += dgvVendorItems_CellClick;

        }

        private void dgvMyItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //The first column of a datagridview has an index of 0
            //The fifth column is the button
            if(e.ColumnIndex == 4)
            {
                //This gets the ID value of the item from the hidden 1st column
                var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;

                //Get the item object for the selected item row
                Item itemBeingSold = World.ItemByID(Convert.ToInt32(itemID));

                if(itemBeingSold.Price == World.UNSELLABLE_ITEM_PRICE)
                {
                    MessageBox.Show("You can not sell the" + itemBeingSold.Name);
                }
                else
                {
                    //REmove one of these items from the players inventory
                    _currentPlayer.RemoveItemFromInventory(itemBeingSold);

                    //give the player the gold for the item being sold
                    _currentPlayer.Gold += itemBeingSold.Price;
                }
            }

        }
    
        private void dgvVendorItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //The 4th column (index 3) has the buy 1 button
            if(e.ColumnIndex == 3)
            {
                var itemID = dgvVendorItems.Rows[e.RowIndex].Cells[0].Value;

                Item itemBeingBought = World.ItemByID(Convert.ToInt32(itemID));

                if(_currentPlayer.Gold >= itemBeingBought.Price)
                {
                    _currentPlayer.AddItemToInventory(itemBeingBought);

                    _currentPlayer.Gold -= itemBeingBought.Price;
                }
                else
                {
                    MessageBox.Show("You dont have enough gold to buy the " + itemBeingBought.Name);
                }
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close(); //Schließt das Formular lul
        }
    }
}
