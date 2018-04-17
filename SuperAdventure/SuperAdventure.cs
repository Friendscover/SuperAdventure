using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Engine;


namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player;   // Variabel Player aus Engine wird genutztas
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        public SuperAdventure()
        {
            InitializeComponent();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer();
            }

            lblHitPoints.DataBindings.Add("Text", _player, "CurrentHitPoints");
            lblGold.DataBindings.Add("Text", _player, "Gold");
            lblExperience.DataBindings.Add("Text", _player, "ExperiencePoints");
            lblLevel.DataBindings.Add("Text", _player, "Level");

            dgvInventory.RowHeadersVisible = false;
            dgvInventory.AutoGenerateColumns = false;

            dgvInventory.DataSource = _player.Inventory;

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "Description"
            });

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Quantity",
                DataPropertyName = "Quantity"
            });

            dgvQuests.RowHeadersVisible = false;
            dgvQuests.AutoGenerateColumns = false;

            dgvQuests.DataSource = _player.Quests;

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "Name"
            });

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Done?",
                DataPropertyName = "IsCompleted"
            });

            cboWeapons.DataSource = _player.Weapons;
            cboWeapons.DisplayMember = "Name";
            cboWeapons.ValueMember = "Id";

            if(_player.CurrentWeapon != null)
            {
                cboWeapons.SelectedItem = _player.CurrentWeapon;
            }

            cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;

            cboPotions.DataSource = _player.Potions;
            cboPotions.DisplayMember = "Name";
            cboPotions.ValueMember = "Id";

            _player.PropertyChanged += PlayerOnPropertyChanged;
            _player.OnMessage += DisplayMessage;

            _player.MoveTo(_player.CurrentLocation);
            
        }

        private void SuperAdventure_Load(object sender, EventArgs e)
        {
            
        }

       
        //Aktualisiert die Location für die Player Klasse wenn der Button für die Richtung geklickt wurde
        private void btnNorth_Click(object sender, EventArgs e)
        {
            _player.MoveNorth();
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            _player.MoveWest();
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            _player.MoveEast();
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            _player.MoveSouth();
        }

    
        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            //Get the currently selected weapon from the cboWeapons Combobox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            _player.UseWeapon(currentWeapon);
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            //Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            _player.UsePotion(potion);
        }

        private void ScrollToBottomOfMessages()
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }
        
        private void UpdatePlayerStats()
        {
            //Refresh player information and inventory controls
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void SuperAdventure_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
        }

        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            _player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;        //durch dars Weapon vor der Object auswahl wird sicher gestellt das das gespeicherte Elemente auch ne Weapon ist
        }

        private void DisplayMessage(object sender, MessageEventArgs messageEventArgs)
        {
            rtbMessages.Text += messageEventArgs.Message + Environment.NewLine;

            if (messageEventArgs.AddExtraNewLine)
            {
                rtbMessages.Text += Environment.NewLine;
            }

            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if(propertyChangedEventArgs.PropertyName == "Weapons")
            {
                cboWeapons.DataSource = _player.Weapons;

                if (!_player.Weapons.Any())
                {
                    cboWeapons.Visible = false;
                    btnUseWeapon.Visible = false;
                }
            }

            if(propertyChangedEventArgs.PropertyName == "Potions")
            {
                cboPotions.DataSource = _player.Potions;

                if (!_player.Potions.Any())
                {
                    cboPotions.Visible = false;
                    btnUsePotion.Visible = false;
                }
            }
            
            if(propertyChangedEventArgs.PropertyName == "CurrentLocation")
            {
                //Show/hide available buttons
                btnNorth.Visible = (_player.CurrentLocation.LocationToNorth != null);
                btnEast.Visible = (_player.CurrentLocation.LocationToEast != null);
                btnSouth.Visible = (_player.CurrentLocation.LocationToSouth != null);
                btnWest.Visible = (_player.CurrentLocation.LocationToWest != null);

                //Display current location name and description
                rtbLocation.Text = _player.CurrentLocation.Name + Environment.NewLine;
                rtbLocation.Text = _player.CurrentLocation.Description + Environment.NewLine;

                if(_player.CurrentLocation.MonsterLivingHere == null)
                {
                    cboWeapons.Visible = false;
                    cboPotions.Visible = false;
                    btnUseWeapon.Visible = false;
                    btnUsePotion.Visible = false;
                }
                else
                {
                    cboWeapons.Visible = _player.Weapons.Any();
                    cboPotions.Visible = _player.Potions.Any();
                    btnUseWeapon.Visible = _player.Weapons.Any();
                    btnUsePotion.Visible = _player.Potions.Any();
                }
            }
        }
    }
}
