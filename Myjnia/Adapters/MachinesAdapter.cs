﻿using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;

namespace Myjnia.Adapters
{
    internal class MachinesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MachinesAdapterClickEventArgs> ItemClick;

        public event EventHandler<MachinesAdapterClickEventArgs> ItemLongClick;

        private List<Status> Machineslist;
        private Status status;

        public MachinesAdapter(List<Status> data)
        {
            Machineslist = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new MachinesAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Machineslist[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as MachinesAdapterViewHolder;
            holder.MachineNameTextView.Text = status.id.ToString();
            holder.MachineDescriptionTextView.Text = status.status.ToString();
            //holder.TextView.Text = items[position];
        }

        public override int ItemCount => Machineslist.Count;

        private void OnClick(MachinesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);

        private void OnLongClick(MachinesAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class MachinesAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public ImageView MachineImageView;

        public TextView MachineNameTextView;
        public TextView MachineDescriptionTextView;

        public MachinesAdapterViewHolder(View itemView, Action<MachinesAdapterClickEventArgs> clickListener,
                            Action<MachinesAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            MachineImageView = (ImageView)itemView.FindViewById(Resource.Id.MachineImageView);
            MachineNameTextView = (TextView)itemView.FindViewById(Resource.Id.MachineNameTextView);
            MachineDescriptionTextView = (TextView)itemView.FindViewById(Resource.Id.MachineDescriptionTextView);
            itemView.Click += (sender, e) => clickListener(new MachinesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MachinesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class MachinesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}