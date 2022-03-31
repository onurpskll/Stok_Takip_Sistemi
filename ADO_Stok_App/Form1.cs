using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADO_Stok_App.Baglantilarim;

namespace ADO_Stok_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection(Baglanti.con);
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            InsertUpdateIslemleri();
        }

        private void InsertUpdateIslemleri()
        {
            DialogResult kayitCntrl = new DialogResult();
            kayitCntrl = MessageBox.Show(txtStokKodu.Text + " numaralı Stok Kodu için kayıt işlemini onaylıyor musunuz?", "Uyarı", MessageBoxButtons.YesNo);

            if (kayitCntrl == DialogResult.Yes)
            {
                try
                {
                    // kayıt varsa update, kayıt yoksa insert
                    ConnectionCntrl(con);

                    try
                    {
                        SqlCommand cmdStokKoduCntrl = new SqlCommand();
                        cmdStokKoduCntrl.CommandText = @"SELECT STOK_KODU as toplam FROM STOK_TBL WHERE STOK_KODU = " + int.Parse(txtStokKodu.Text);
                        cmdStokKoduCntrl.Connection = con;
                        // kayıt sayısı kontrol et
                        int degerCntrl = cmdStokKoduCntrl.ExecuteScalar() == null ? 0 : 1;

                        if (degerCntrl > 0)
                        {
                            // stok kodu mevcutsa uyarı ver ve soruyla beraber update et
                            MessageBox.Show("Bu stok kodu ile bir kayıt mevcut!");

                            DialogResult updateSoru = MessageBox.Show("Kaydı güncellemek istiyor musunuz?", "GÜNCELLEME", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (updateSoru == DialogResult.Yes)
                            {
                                // update sorgusu
                                StokUpdate();
                            }
                            
                        }
                        else
                        {
                            // stok kodu mevcut değilse insert et
                            StokInsert();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Hata! Bilgi Teknolojileri ile görüşünüz.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    con.Dispose();
                    con.Close();
                }
            }
            else
            {
                return;
            }
        }

        private void CleanStockInfo()
        {
            txtStokKodu.Clear();
            txtStokAdi.Clear();
            txtKategori.Clear();
            txtMiktar.Clear();
            txtBirimFiyat.Clear();
        }

        void HarfKontrol(KeyPressEventArgs e)
        {
            e.Handled = !char.IsNumber(e.KeyChar);
        }

        void SayiKontrol(KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar);
        }

        private SqlConnection ConnectionCntrl(SqlConnection con)
        {       
            if (con.State == ConnectionState.Closed)
            {
                con.ConnectionString = Baglanti.con;
                con.Open();                
            }
            return con;
        }

        private void StokInsert()
        {            
            SqlCommand cmdInsert = new SqlCommand();
            cmdInsert.CommandText = "INSERT INTO STOK_TBL(STOK_KODU,STOK_ADI,MIKTAR,BIRIM_FIYAT,KATEGORI) VALUES (@STOK_KODU,@STOK_ADI,@MIKTAR,@BIRIM_FIYAT,@KATEGORI)";
            cmdInsert.Connection = con;
            cmdInsert.Parameters.AddWithValue("@STOK_KODU", int.Parse(txtStokKodu.Text));
            cmdInsert.Parameters.AddWithValue("@STOK_ADI", txtStokAdi.Text);
            cmdInsert.Parameters.AddWithValue("@MIKTAR", int.Parse(txtMiktar.Text));
            cmdInsert.Parameters.AddWithValue("@BIRIM_FIYAT", int.Parse(txtBirimFiyat.Text));
            cmdInsert.Parameters.AddWithValue("@KATEGORI", txtKategori.Text);
            int kayitDurumCntrl = cmdInsert.ExecuteNonQuery();
            if (kayitDurumCntrl == 1) // kayıt okeyse
            {
                MessageBox.Show(txtStokAdi.Text + " isimli stok sisteme başarıyla kaydedilmiştir", "KAYIT DURUMU");

                CleanStockInfo();
            }
        }

        private void StokUpdate()
        {   
            SqlCommand cmdUpdate = new SqlCommand();
            cmdUpdate.CommandText = @"UPDATE STOK_TBL SET STOK_KODU = @STOK_KODU, STOK_ADI = @STOK_ADI, MIKTAR = @MIKTAR, BIRIM_FIYAT = @BIRIM_FIYAT, KATEGORI = @KATEGORI WHERE STOK_KODU = " + int.Parse(txtStokKodu.Text);
            cmdUpdate.Connection = con;
            cmdUpdate.Parameters.AddWithValue("@STOK_KODU", int.Parse(txtStokKodu.Text));
            cmdUpdate.Parameters.AddWithValue("@STOK_ADI", txtStokAdi.Text);
            cmdUpdate.Parameters.AddWithValue("@MIKTAR", int.Parse(txtMiktar.Text));
            cmdUpdate.Parameters.AddWithValue("@BIRIM_FIYAT", int.Parse(txtBirimFiyat.Text));
            cmdUpdate.Parameters.AddWithValue("@KATEGORI", txtKategori.Text);
            cmdUpdate.ExecuteNonQuery();
            MessageBox.Show(int.Parse(txtStokKodu.Text) + " isimli stok başarıyla güncellenmiştir", "GÜNCELLEME DURUMU");
            CleanStockInfo();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            StokSil();
            GridGuncelle();
        }

        private void StokSil()
        {
            ConnectionCntrl(con);

            int secilenStokId = Convert.ToInt32(dgwStok.CurrentRow.Cells[0].Value);
            SqlCommand cmdDelete = new SqlCommand();
            cmdDelete.CommandText = "DELETE FROM STOK_TBL WHERE STOK_ID =  " + secilenStokId;
            cmdDelete.Connection = con;
            cmdDelete.ExecuteNonQuery();
        }

        private void btnGetir_Click(object sender, EventArgs e)
        {
            StokKodunaGoreGetir();
        }

        void StokKodunaGoreGetir()
        {
            ConnectionCntrl(con);

            try
            {
                SqlDataAdapter da = new SqlDataAdapter($@"SELECT STOK_ID AS STOK_ID, STOK_KODU AS STOK_KODU, STOK_ADI AS STOK_ADI, MIKTAR AS MIKTAR, BIRIM_FIYAT AS BIRIM_FIYAT, KATEGORI AS KATEGORI, (BIRIM_FIYAT * MIKTAR) AS NET_FIYAT FROM STOK_TBL WHERE STOK_KODU = {int.Parse(txtStokKodu.Text)} ", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count < 1)
                {
                    MessageBox.Show($"Stokta {txtStokKodu.Text} isimli stok kodu bulunmamaktadır!");
                    return;
                }
                else
                {
                    dgwStok.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lütfen Stok Kodu Giriniz!");
                txtStokKodu.Focus();
            }
        }

        private void GridGuncelle()
        {
            ConnectionCntrl(con);
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT STOK_ID as STOK_ID, STOK_KODU as STOK_KODU, STOK_ADI as STOK_ADI, MIKTAR as MIKTAR, BIRIM_FIYAT as BIRIM_FIYAT, KATEGORI as KATEGORI FROM STOK_TBL", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgwStok.DataSource = dt;
        }

        private void dgwStok_SelectionChanged(object sender, EventArgs e)
        {
            GridSecileniRenklendir();
        }

        private void GridSecileniRenklendir()
        {
            dgwStok.DefaultCellStyle.SelectionBackColor = Color.Khaki;
            dgwStok.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void txtStokKodu_KeyPress(object sender, KeyPressEventArgs e)
        {
            HarfKontrol(e);
        }

        private void txtStokAdi_KeyPress(object sender, KeyPressEventArgs e)
        {
            SayiKontrol(e);
        }

        private void txtMiktar_KeyPress(object sender, KeyPressEventArgs e)
        {
            HarfKontrol(e);
        }

        private void txtBirimFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            HarfKontrol(e);
        }

        private void txtKategori_KeyPress(object sender, KeyPressEventArgs e)
        {
            SayiKontrol(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GridGuncelle();
        }

        private void btnYenile_Click(object sender, EventArgs e)
        {          
            GridGuncelle();
        }
    }
}
