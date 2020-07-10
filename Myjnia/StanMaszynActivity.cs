using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Myjnia.Adapters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Myjnia
{
    [Activity(Label = "StanMaszynActivity")]
    public class StanMaszynActivity : Activity
    {
        private RecyclerView MachinesRecyclerView;

        private MachinesAdapter machinesAdapter;
        private List<Status> listOfMachines;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StanMaszyn);
            // Create your application here
            MachinesRecyclerView = (RecyclerView)FindViewById(Resource.Id.MachinesRecyclerView);
            await Send();
            SetupRecyclerView();
        }

        private async Task Send()
        {
            try
            {
                var url = "http://80.211.242.184/machine/machineStatus";
                using var client = new HttpClient();

                var response = await client.GetAsync(url);
                Toast.MakeText(this, response.StatusCode.ToString(), ToastLength.Short).Show();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    //var resultObject = JObject.Parse(result);
                    //listOfMachines = new List<Status>();
                    //string jsonList = string.Empty;
                    //var dList = JsonConvert.DeserializeObject<List<Root>>(result);
                    var des = (Root)JsonConvert.DeserializeObject(result, typeof(Root));
                    //return des.data.Count.ToString();
                    //Toast.MakeText(this, des.status.Count.ToString(), ToastLength.Long).Show();

                    listOfMachines.AddRange(des.status);
                }
                else
                {
                    string blad = "Brak połączenia z serwerem! kod: " + response.StatusCode;
                    var toast = Toast.MakeText(this, blad, ToastLength.Short);
                    toast.Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long);
                Log.Info("blad", ex.ToString());
            }
        }

        private void SetupRecyclerView()
        {
            MachinesRecyclerView.SetLayoutManager(new LinearLayoutManager(MachinesRecyclerView.Context));
            machinesAdapter = new MachinesAdapter(listOfMachines);
            MachinesRecyclerView.SetAdapter(machinesAdapter);
        }
    }
}