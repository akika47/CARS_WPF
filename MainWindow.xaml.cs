using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CARS_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string connectionString = "server=localhost;database=classicmodels;user=root;password=;";

    public MainWindow()
    {
        InitializeComponent();
        LoadProducts();
        LoadCountries();
        LoadOrders();
    }

    private void LoadProducts()
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT productCode, productName FROM products";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            lbProducts.ItemsSource = dt.DefaultView;
        }
    }

    private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lbProducts.SelectedItem is DataRowView row)
        {
            string productCode = row["productCode"].ToString();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM orderdetails WHERE productCode = @productCode";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@productCode", productCode);
                int orderCount = Convert.ToInt32(cmd.ExecuteScalar());

                if (orderCount == 0)
                {
                    MessageBox.Show("Nincs rendelés erre a termékre.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    lblOrderCount.Content = $"Rendelések száma: {orderCount}";
                }
            }
        }
    }

    private void LoadCountries()
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT DISTINCT country FROM customers ORDER BY country";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            cbxCountry.ItemsSource = dt.DefaultView;
            cbxCountry.DisplayMemberPath = "country";
        }
    }

    private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbxCountry.SelectedItem is DataRowView row)
        {
            string country = row["country"].ToString();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT customerName, phone, city FROM customers WHERE country = @country";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@country", country);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dtgCustomers.ItemsSource = dt.DefaultView;
            }
        }
    }

    private void LoadOrders()
    {

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT orderNumber, orderDate, status FROM orders";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dtgOrders.ItemsSource = dt.DefaultView;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }

    private void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dtgOrders.SelectedItem is DataRowView row)
        {
            int orderNumber = Convert.ToInt32(row["orderNumber"]);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT p.productName, p.buyPrice FROM orderdetails od JOIN products p ON od.productCode = p.productCode WHERE od.orderNumber = @orderNumber ORDER BY p.productName";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@orderNumber", orderNumber);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                lbOrderedProducts.ItemsSource = dt.DefaultView;
            }
        }
    }
}