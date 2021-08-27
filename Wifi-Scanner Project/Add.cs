using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class Add
    {
        
        public void add(ListView listview2, ListView listview1)
        {

            listview2.Items.Add(listview1.SelectedItems[0].Text);
            
            
            
            
        }
        
    }
}
