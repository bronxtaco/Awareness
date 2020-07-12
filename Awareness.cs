using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Awareness
{
    struct Item
    {
        public string Name;
        public Slot Slot;
        public string ImagePath;
        public int ImageW;
        public int ImageH;
    }
    enum Slot
    {
        Shovel,
        Weapon,
        Body,
        Head,
        Feet,
        Torch,
        Ring,
        Misc,
        Spell,
        Item,
        Hud,
    }
    public partial class Awareness : Form
    {
        static string ND_XMLPATH = Application.StartupPath + @"\necrodancer.xml";
        static string ND_ITEMPATH = Application.StartupPath.ToLower().Replace("crypt of the necrodancer\\mods\\awareness", "Crypt of the NecroDancer\\data\\items");
        static string ND_GUIPATH = Application.StartupPath.ToLower().Replace("crypt of the necrodancer\\mods\\awareness", "Crypt of the NecroDancer\\data\\gui");
        static string ND_LEVELPATH = Application.StartupPath.ToLower().Replace("crypt of the necrodancer\\mods\\awareness", "Crypt of the NecroDancer\\data\\level");

        XmlDocument xml = new XmlDocument();
        XmlNode cadence;

        ArrayList allItems = new ArrayList();

        Dictionary<string, Slot> itemSubstrings = new Dictionary<string, Slot>()
        {
            { "shovel_", Slot.Shovel },
            { "pickaxe", Slot.Shovel },
            { "weapon_", Slot.Weapon },
            { "armor_", Slot.Body },
            { "head_", Slot.Head },
            { "feet_", Slot.Feet },
            { "torch_", Slot.Torch },
            { "ring_", Slot.Ring },
            { "misc_", Slot.Misc },
            { "charm_", Slot.Misc },
            { "spell_", Slot.Spell },
            { "cursed_", Slot.Item },
            { "food_", Slot.Item },
            { "holy_", Slot.Item },
            { "_drum", Slot.Item },
            { "heart_trans", Slot.Item },
            { "scroll_", Slot.Item },
            { "throwing_", Slot.Item },
            { "familiar_", Slot.Item },
            { "tome_", Slot.Item },
            { "hud_", Slot.Hud },
            { "holster", Slot.Hud },
            { "bag_", Slot.Hud },
        };
        
        Dictionary<Slot, string> slotImagePaths = new Dictionary<Slot, string>()
        {
            { Slot.Shovel, (ND_GUIPATH + @"\hud_slot_1.png") },
            { Slot.Weapon, (ND_GUIPATH + @"\hud_slot_2.png") },
            { Slot.Body, (ND_GUIPATH + @"\hud_slot_3.png") },
            { Slot.Head, (ND_GUIPATH + @"\hud_slot_4.png") },
            { Slot.Feet, (ND_GUIPATH + @"\hud_slot_5.png") },
            { Slot.Torch, (ND_GUIPATH + @"\hud_slot_6.png") },
            { Slot.Ring, (ND_GUIPATH + @"\hud_slot_7.png") },
            { Slot.Spell, (ND_GUIPATH + @"\hud_slot_spell1.png") },
            { Slot.Item, (ND_GUIPATH + @"\hud_slot_action1.png") },
        };

        Dictionary<string, bool> miscBools = new Dictionary<string, bool>()
        {
            { "misc_compass", false },
            { "misc_coupon", false },
            { "misc_map", false },
            { "misc_monkey_paw", false },
            { "misc_potion", false },
            { "charm_bomb", false },
            { "charm_frost", false },
            { "charm_gluttony", false },
            { "charm_grenade", false },
            { "charm_luck", false },
            { "charm_nazar", false },
            { "charm_protection", false },
            { "charm_risk", false },
            { "charm_strength", false },
        };

        string chanceImagePath = (ND_LEVELPATH + @"\shrine_chance.png");
        bool chanceActive = false;
        string seedImagePath = (ND_LEVELPATH + @"\stairs.png");

        int activeSpellSlot = 1;

        public Awareness()
        {
            InitializeComponent();
        }

        private void Awareness_Load(object sender, EventArgs e)
        {
            // reset form control settings
            this.SetBounds(0, 0, 850, 310);

            int pnlX = 80;
            int pnlY = 103;
            int pnlW = 669;
            int pnlH = 246;
            pnl_shovel.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_body.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_head.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_feet.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_torch.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_ring.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_spell.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_item_menu.SetBounds(pnlX, pnlY, pnlW, pnlH);

            pnl_weapon_dagger.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_spear.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_broadsword.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_longsword.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_whip.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_rapier.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_bow.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_crossbow.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_flail.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_cat.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_axe.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_harp.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_warhammer.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_staff.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_weapon_cutlass.SetBounds(pnlX, pnlY, pnlW, pnlH);

            pnl_item_food.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_item_scroll.SetBounds(pnlX, pnlY, pnlW, pnlH);
            pnl_item_misc.SetBounds(pnlX, pnlY, pnlW, pnlH);

            pnl_seed.SetBounds(pnlX, pnlY, pnlW, pnlH);

            xml.Load(ND_XMLPATH);
            XmlNodeList characters = xml.SelectNodes("//necrodancer/characters/character");

            foreach (XmlNode character in characters)
            {
                foreach (XmlAttribute attribute in character.Attributes)
                {
                    if (int.Parse(attribute.Value) == 0)
                    {
                        cadence = character.SelectSingleNode("//initial_equipment");
                        foreach (XmlNode itemNode in cadence)
                        {
                            Console.WriteLine(itemNode.Attributes["type"].Value);
                        }
                    }
                }
            }
            var slots = Enum.GetValues(typeof(Slot)).Cast<Slot>();
            foreach (Slot slot in slots)
            {
                Item itemNone = new Item();
                itemNone.Name = "none_" + slot.ToString();
                itemNone.Slot = slot;
                allItems.Add(itemNone);
            }
            
            XmlNode itemsNode = xml.SelectSingleNode("//necrodancer/items");
            foreach (XmlNode itemNode in itemsNode)
            {
                // early outs
                // xml comments
                if (itemNode.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                // not slot items
                if (itemNode.Attributes["slot"] == null)
                {
                    continue;
                }
                // levelEditor = False
                if (itemNode.Attributes["levelEditor"] != null && itemNode.Name != "ring_wonder")
                {
                    continue;
                }
                // bombs
                if (itemNode.Attributes["slot"].Value == "bomb")
                {
                    continue;
                }
                // specific omissions
                if (itemNode.Name == "weapon_flower" ||
                    itemNode.Name == "armor_platemail_dorian" || 
                    itemNode.Name == "head_ninja_mask" ||
                    itemNode.Name == "head_sonar" ||
                    itemNode.Name == "feet_boots_speed" ||
                    itemNode.Name == "ring_phasing" ||
                    itemNode.Name == "misc_key" ||
                    itemNode.Name == "familiar_shield")
                {
                    continue;
                }
                Item item = new Item();
                item.Name = itemNode.Name;
                foreach (KeyValuePair<string, Slot> itemSubstring in itemSubstrings)
                {
                    if (item.Name.Contains(itemSubstring.Key))
                    {
                        item.Slot = itemSubstring.Value;
                    }
                }
                item.ImagePath = (ND_ITEMPATH + @"\" + itemNode.InnerText);
                item.ImageW = 24;
                item.ImageH = 24;
                if (itemNode.Attributes["imageW"] != null)
                {
                    item.ImageW = int.Parse(itemNode.Attributes["imageW"].Value);
                }
                if (itemNode.Attributes["imageH"] != null)
                {
                    item.ImageH = int.Parse(itemNode.Attributes["imageH"].Value);
                }
                allItems.Add(item);
            }
            updateMainButtons();
        }
        //-------------------------Get specific item from allItems
        private Item getItem(string name)
        {
            foreach (Item item in allItems)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return new Item();
        }

        //-------------------------Update main edge info
        private void updateMainButtons()
        {
            // set all buttons to 'none'
            btn_shovel.Text = "none_" + Slot.Shovel.ToString();
            btn_weapon.Text = "none_" + Slot.Weapon.ToString();
            btn_body.Text = "none_" + Slot.Body.ToString();
            btn_head.Text = "none_" + Slot.Head.ToString();
            btn_feet.Text = "none_" + Slot.Feet.ToString();
            btn_torch.Text = "none_" + Slot.Torch.ToString();
            btn_ring.Text = "none_" + Slot.Ring.ToString();
            btn_spell1.Text = "none_" + Slot.Spell.ToString();
            btn_spell2.Text = "none_" + Slot.Spell.ToString();
            btn_spell2.Visible = false;
            btn_item1.Text = "none_" + Slot.Item.ToString();

            // reset misc
            miscBools["misc_compass"] = false;
            miscBools["misc_coupon"] = false;
            miscBools["misc_map"] = false;
            miscBools["misc_monkey_paw"] = false;
            miscBools["misc_potion"] = false;
            miscBools["charm_bomb"] = false;
            miscBools["charm_frost"] = false;
            miscBools["charm_gluttony"] = false;
            miscBools["charm_grenade"] = false;
            miscBools["charm_luck"] = false;
            miscBools["charm_nazar"] = false;
            miscBools["charm_protection"] = false;
            miscBools["charm_risk"] = false;
            miscBools["charm_strength"] = false;

            // update the text and miscBool values
            foreach (XmlNode itemNode in cadence)
            {
                Item item = getItem(itemNode.Attributes["type"].Value);
                switch (item.Slot)
                {
                    case Slot.Shovel:
                        btn_shovel.Text = item.Name;
                        break;
                    case Slot.Weapon:
                        btn_weapon.Text = item.Name;
                        break;
                    case Slot.Body:
                        btn_body.Text = item.Name;
                        break;
                    case Slot.Head:
                        btn_head.Text = item.Name;
                        break;
                    case Slot.Feet:
                        btn_feet.Text = item.Name;
                        break;
                    case Slot.Torch:
                        btn_torch.Text = item.Name;
                        break;
                    case Slot.Ring:
                        btn_ring.Text = item.Name;
                        break;
                    case Slot.Spell:
                        btn_spell2.Visible = true;
                        if (btn_spell1.Text.Contains("none"))
                        {
                            btn_spell1.Text = item.Name;
                        }
                        else
                        {
                            btn_spell2.Text = item.Name;
                        }
                        break;
                    case Slot.Item:
                        btn_item1.Text = item.Name;
                        break;
                    case Slot.Misc:
                        miscBools[item.Name] = true;
                        break;
                    default:
                        break;
                }
            }
            this.Refresh();
        }

        private void btnMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // slot background
            Button btn = (Button)sender;
            Item item = getItem(btn.Text);
            int rectW = 62;
            int rectH = 68;
            int imgW = 30;
            int imgH = 33;
            if (item.Slot == Slot.Item || item.Slot == Slot.Spell)
            {
                rectW = 66;
                rectH = 88;
                imgW = 32;
                imgH = 42;
            }
            Image bgImage = new Bitmap(slotImagePaths[item.Slot]);
            Rectangle bgRect = new Rectangle(0, 0, rectW, rectH);
            e.Graphics.DrawImage(bgImage, bgRect, 0, 0, imgW, imgH, GraphicsUnit.Pixel);

            // item
            if (item.Name.Contains("none_")) { return; }
            drawItemGfx(item, btn, e);
        }

        //-------------------------Update panel button info
        private void btnPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // item
            Button btn = (Button)sender;
            Item item = getItem(btn.Text);
            drawItemGfx(item, btn, e);
        }

        private void drawItemGfx(Item item, Button btn, PaintEventArgs e)
        {
            int baseWidth = item.ImageW;
            int baseHeight = item.ImageH;
            int scaledWidth = baseWidth * 2;
            int scaledHeight = baseHeight * 2;
            int imgX = (int)((btn.Size.Width - scaledWidth) / 2);
            int imgY = (int)((btn.Size.Height - scaledHeight) / 2) + 2;

            Rectangle rect = new Rectangle(imgX, imgY, scaledWidth, scaledHeight);
            Image image = new Bitmap(item.ImagePath);
            e.Graphics.DrawImage(image, rect, 0, 0, baseWidth - 1, baseHeight, GraphicsUnit.Pixel);
        }

        //-------------------------Update misc button info
        private void btnMisc_Paint(object sender, PaintEventArgs e)
        {
            // misc item
            Button btn = (Button)sender;
            Item item = getItem(btn.Text);
            bool active = miscBools[item.Name];
            ImageAttributes imgAtt = new ImageAttributes();
            if (!active)
            {
                float[][] colorMatrixElements = {
                    new float[] {0,  0,  0,  0, 0},        // red scaling factor of 2
                    new float[] {0,  0,  0,  0, 0},        // green scaling factor of 1
                    new float[] {0,  0,  0,  0, 0},        // blue scaling factor of 1
                    new float[] {0,  0,  0,  1, 0},        // alpha scaling factor of 1
                    new float[] {.2f, .2f, .2f, 0, 1}    // three translations of 0.2
                };
                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                imgAtt.SetColorMatrix(
                    colorMatrix,
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap
                );
            }
            int baseWidth = item.ImageW;
            int baseHeight = item.ImageH;
            int imgX = (int)((btn.Size.Width - baseWidth) / 2);
            int imgY = (int)((btn.Size.Height - baseHeight) / 2);

            Rectangle rect = new Rectangle(imgX, imgY, baseWidth, baseHeight);
            Image image = new Bitmap(item.ImagePath);
            e.Graphics.DrawImage(image, rect, 0, 0, baseWidth, baseHeight, GraphicsUnit.Pixel, imgAtt);
        }

        //-------------------------Chance button paint
        private void btnChance_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            Button btn = (Button)sender;
            Item item = getItem(btn.Text);
            int rectW = 72;
            int rectH = 108;
            int imgW = 36;
            int imgH = 54;
            int drawX = 0;
            if (chanceActive)
            {
                drawX = imgW-1;
            }
            Image bgImage = new Bitmap(chanceImagePath);
            Rectangle bgRect = new Rectangle(0, -35, rectW, rectH);
            e.Graphics.DrawImage(bgImage, bgRect, drawX, 0, imgW, imgH, GraphicsUnit.Pixel);
        }

        //-------------------------Seed button paint
        private void btnSeed_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            Button btn = (Button)sender;
            Item item = getItem(btn.Text);
            int rectW = 48;
            int rectH = 48;
            int imgW = 24;
            int imgH = 24;
            int drawX = 0;
            if (chanceActive)
            {
                drawX = imgW - 1;
            }
            Image bgImage = new Bitmap(seedImagePath);
            Rectangle bgRect = new Rectangle(12, 18, rectW, rectH);
            e.Graphics.DrawImage(bgImage, bgRect, drawX, 0, imgW, imgH, GraphicsUnit.Pixel);
        }

        //-------------------------Main edge button clicks
        private void clickPanel(Panel pnl)
        {
            chanceActive = false;
            if (pnl.Visible)
            {
                pnl.Visible = false;
            }
            else
            {
                pnl_shovel.Visible = false;
                pnl_weapon.Visible = false;
                pnl_body.Visible = false;
                pnl_head.Visible = false;
                pnl_feet.Visible = false;
                pnl_torch.Visible = false;
                pnl_ring.Visible = false;
                pnl_spell.Visible = false;
                pnl_item_menu.Visible = false;

                pnl_weapon_dagger.Visible = false;
                pnl_weapon_spear.Visible = false;
                pnl_weapon_broadsword.Visible = false;
                pnl_weapon_longsword.Visible = false;
                pnl_weapon_whip.Visible = false;
                pnl_weapon_rapier.Visible = false;
                pnl_weapon_bow.Visible = false;
                pnl_weapon_crossbow.Visible = false;
                pnl_weapon_flail.Visible = false;
                pnl_weapon_cat.Visible = false;
                pnl_weapon_axe.Visible = false;
                pnl_weapon_harp.Visible = false;
                pnl_weapon_warhammer.Visible = false;
                pnl_weapon_staff.Visible = false;
                pnl_weapon_cutlass.Visible = false;

                pnl_item_food.Visible = false;
                pnl_item_scroll.Visible = false;
                pnl_item_misc.Visible = false;

                pnl_seed.Visible = false;

                pnl.Visible = true;
            }
        }

        private void btn_shovel_Click(object sender, EventArgs e)
        {
            if (pnl_shovel.Visible)
            {
                updateItem(getItem("none_Shovel"), getItem(btn_shovel.Text), pnl_shovel);
            }
            else
            {
                clickPanel(pnl_shovel);
            }
        }

        private void btn_weapon_Click(object sender, EventArgs e)
        {
            if (pnl_weapon.Visible)
            {
                updateItem(getItem("none_Weapon"), getItem(btn_weapon.Text), pnl_weapon);
            }
            else
            {
                clickPanel(pnl_weapon);
            }
        }

        private void btn_body_Click(object sender, EventArgs e)
        {
            if (pnl_body.Visible)
            {
                updateItem(getItem("none_Body"), getItem(btn_body.Text), pnl_body);
            }
            else
            {
                clickPanel(pnl_body);
            }
        }

        private void btn_head_Click(object sender, EventArgs e)
        {
            if (pnl_head.Visible)
            {
                updateItem(getItem("none_Head"), getItem(btn_head.Text), pnl_head);
            }
            else
            {
                clickPanel(pnl_head);
            }
        }

        private void btn_feet_Click(object sender, EventArgs e)
        {
            if (pnl_feet.Visible)
            {
                updateItem(getItem("none_Feet"), getItem(btn_feet.Text), pnl_feet);
            }
            else
            {
                clickPanel(pnl_feet);
            }
        }

        private void btn_torch_Click(object sender, EventArgs e)
        {
            if (pnl_torch.Visible)
            {
                updateItem(getItem("none_Torch"), getItem(btn_torch.Text), pnl_torch);
            }
            else
            {
                clickPanel(pnl_torch);
            }
        }

        private void btn_ring_Click(object sender, EventArgs e)
        {
            if (pnl_ring.Visible)
            {
                updateItem(getItem("none_Ring"), getItem(btn_ring.Text), pnl_ring);
            }
            else
            {
                clickPanel(pnl_ring);
            }
        }

        private void btn_spell1_Click(object sender, EventArgs e)
        {
            activeSpellSlot = 1;
            if (pnl_spell.Visible)
            {
                updateItem(getItem("none_Spell"), getItem(btn_spell1.Text), pnl_spell);
            }
            else
            {
                clickPanel(pnl_spell);
            }
        }

        private void btn_spell2_Click(object sender, EventArgs e)
        {
            activeSpellSlot = 2;
            if (pnl_spell.Visible)
            {
                updateItem(getItem("none_Spell"), getItem(btn_spell2.Text), pnl_spell);
            }
            else
            {
                clickPanel(pnl_spell);
            }
        }

        private void btn_item1_Click(object sender, EventArgs e)
        {
            if (pnl_item_menu.Visible)
            {
                updateItem(getItem("none_Item"), getItem(btn_item1.Text), pnl_item_menu);
            }
            else
            {
                clickPanel(pnl_item_menu);
            }
        }

        //-------------------------Weapon menu clicks
        private void btn_open_menu_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Text)
            {
                case "weapon_dagger":
                    clickPanel(pnl_weapon_dagger);
                    break;
                case "weapon_spear":
                    clickPanel(pnl_weapon_spear);
                    break;
                case "weapon_broadsword":
                    clickPanel(pnl_weapon_broadsword);
                    break;
                case "weapon_longsword":
                    clickPanel(pnl_weapon_longsword);
                    break;
                case "weapon_whip":
                    clickPanel(pnl_weapon_whip);
                    break;
                case "weapon_rapier":
                    clickPanel(pnl_weapon_rapier);
                    break;
                case "weapon_bow":
                    clickPanel(pnl_weapon_bow);
                    break;
                case "weapon_crossbow":
                    clickPanel(pnl_weapon_crossbow);
                    break;
                case "weapon_flail":
                    clickPanel(pnl_weapon_flail);
                    break;
                case "weapon_cat":
                    clickPanel(pnl_weapon_cat);
                    break;
                case "weapon_axe":
                    clickPanel(pnl_weapon_axe);
                    break;
                case "weapon_harp":
                    clickPanel(pnl_weapon_harp);
                    break;
                case "weapon_warhammer":
                    clickPanel(pnl_weapon_warhammer);
                    break;
                case "weapon_staff":
                    clickPanel(pnl_weapon_staff);
                    break;
                case "weapon_cutlass":
                    clickPanel(pnl_weapon_cutlass);
                    break;
                case "food_1":
                    clickPanel(pnl_item_food);
                    break;
                case "scroll_fireball":
                    clickPanel(pnl_item_scroll);
                    break;
                case "heart_transplant":
                    clickPanel(pnl_item_misc);
                    break;
                default:
                    break;
            }
        }

        //-------------------------Misc button clicks
        private void btn_misc_Click(object sender, EventArgs e)
        {
            chanceActive = false;
            Button btn = (Button)sender;
            miscBools[btn.Text] = !miscBools[btn.Text];
            updateMiscItem(btn.Text);
        }

        //-------------------------Generate random build
        private void generateBuild(long seed)
        {
            JavaRng rng = new JavaRng(seed);

            // remove all items
            cadence.RemoveAll();

            // add one random item for each slot
            addRandomItem(seed, Slot.Shovel);
            addRandomItem(seed, Slot.Weapon);
            addRandomItem(seed, Slot.Body);
            addRandomItem(seed, Slot.Head);
            addRandomItem(seed, Slot.Feet);
            addRandomItem(seed, Slot.Torch);
            addRandomItem(seed, Slot.Ring);
            addRandomItem(seed, Slot.Spell);
            addRandomItem(seed, Slot.Item);
            addRandomMiscItems(seed);

            xml.Save(ND_XMLPATH);
            updateMainButtons();
        }

        //-------------------------Chance click
        private void btn_chance_Click(object sender, EventArgs e)
        {
            generateBuild(DateTime.Now.Millisecond);
            chanceActive = true;
        }

        //-------------------------Seed click
        private void btn_seed_Click(object sender, EventArgs e)
        {
            clickPanel(pnl_seed);
        }

        private void addRandomItem(long seed, Slot slot)
        {
            JavaRng rng = new JavaRng(seed);
            ArrayList slotItems = new ArrayList();
            foreach (Item item in allItems)
            {
                if (item.Slot == slot && !item.Name.Contains("none_"))
                {
                    slotItems.Add(item);
                }
            }
            int rndIndex = rng.NextInt(slotItems.Count);
            Item rndItem = (Item)slotItems[rndIndex];

            XmlElement newItem = xml.CreateElement("item");
            XmlAttribute newType = xml.CreateAttribute("type");
            newType.Value = rndItem.Name;
            newItem.SetAttributeNode(newType);
            cadence.AppendChild(newItem);
        }

        private void addRandomMiscItems(long seed)
        {
            JavaRng rng = new JavaRng(seed);
            foreach (var miscItem in miscBools.ToArray())
            {
                if (rng.NextInt(4) == 1)
                {
                    miscBools[miscItem.Key] = true;
                    updateMiscItem(miscItem.Key);
                }
            }
        }

        //-------------------------Add item and update xml
        private void updateItem(Item addItem, Item remItem, Panel pnl)
        {
            if (addItem.Slot != remItem.Slot)
            {
                Console.WriteLine("Item to add different slot to item to remove!");
                return;
            }
            foreach (XmlNode itemNode in cadence)
            {
                if (itemNode.Attributes["type"].Value == remItem.Name)
                {
                    cadence.RemoveChild(itemNode);
                    break;
                }
            }
            
            if (!addItem.Name.Contains("none_"))
            {
                XmlElement newItem = xml.CreateElement("item");
                XmlAttribute newType = xml.CreateAttribute("type");
                newType.Value = addItem.Name;
                newItem.SetAttributeNode(newType);

                if (activeSpellSlot == 1 && addItem.Slot == Slot.Spell && !btn_spell2.Text.Contains("none"))
                {
                    foreach (XmlNode itemNode in cadence)
                    {
                        if (itemNode.Attributes["type"].Value.Contains("spell_"))
                        {
                            cadence.InsertBefore(newItem, itemNode);
                            break;
                        }
                    }
                }
                else
                {
                    cadence.AppendChild(newItem);
                }
            }

            clickPanel(pnl);
            xml.Save(ND_XMLPATH);
            updateMainButtons();
        }

        private void updateMiscItem(string miscName)
        {
            if (miscBools[miscName])
            {
                XmlElement newItem = xml.CreateElement("item");
                XmlAttribute newType = xml.CreateAttribute("type");
                newType.Value = miscName;
                newItem.SetAttributeNode(newType);
                cadence.AppendChild(newItem);
            }
            else
            {
                foreach (XmlNode itemNode in cadence)
                {
                    if (itemNode.Attributes["type"].Value == miscName)
                    {
                        cadence.RemoveChild(itemNode);
                        break;
                    }
                }
            }
            xml.Save(ND_XMLPATH);
            updateMainButtons();
        }

        //-------------------------Select item buttons
        private void btn_select_shovel_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            updateItem(getItem(btn.Text), getItem(btn_shovel.Text), (Panel)btn.Parent);
        }

        private void btn_select_weapon_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            updateItem(getItem(btn.Text), getItem(btn_weapon.Text), (Panel)btn.Parent);
        }

        private void btn_select_body_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            updateItem(getItem(btn.Text), getItem(btn_body.Text), (Panel)btn.Parent);
        }

        private void btn_select_head_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            updateItem(getItem(btn.Text), getItem(btn_head.Text), (Panel)btn.Parent);
        }

        private void btn_select_feet_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            updateItem(getItem(btn.Text), getItem(btn_feet.Text), (Panel)btn.Parent);
        }

        private void btn_select_torch_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            updateItem(getItem(btn.Text), getItem(btn_torch.Text), (Panel)btn.Parent);
        }

        private void btn_select_ring_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            updateItem(getItem(btn.Text), getItem(btn_ring.Text), (Panel)btn.Parent);
        }

        private void btn_select_spell_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (activeSpellSlot == 1)
            {
                if (btn.Text != btn_spell2.Text)
                {
                    updateItem(getItem(btn.Text), getItem(btn_spell1.Text), (Panel)btn.Parent);
                }
            }
            else if (activeSpellSlot == 2)
            {
                if (btn.Text != btn_spell1.Text)
                {
                    updateItem(getItem(btn.Text), getItem(btn_spell2.Text), (Panel)btn.Parent);
                }
            }
        }

        private void btn_select_item_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            updateItem(getItem(btn.Text), getItem(btn_item1.Text), (Panel)btn.Parent);
        }

        private void txt_seed_TextChanged(object sender, EventArgs e)
        {
            if (txt_seed.Text != "")
            {
                string seedText = Regex.Replace(txt_seed.Text, "[^.0-9]", "");
                txt_seed.Text = seedText;
                if (txt_seed.Text != "")
                {
                    generateBuild(Int64.Parse(txt_seed.Text));
                }
            }
        }
    }
}