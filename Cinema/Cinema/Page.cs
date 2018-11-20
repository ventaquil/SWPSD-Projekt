using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cinema
{
    public partial class Page : System.Windows.Controls.Page
    {
        private readonly Page previousPage;

        private readonly SqlConnection sqlConnection;

        private readonly Window window;

        public Page(Window window, SqlConnection sqlConnection) : this(window, null, sqlConnection)
        {
        }

        public Page(Window window, Page previousPage, SqlConnection sqlConnection)
        {
            this.window = window;

            this.previousPage = previousPage;

            this.sqlConnection = sqlConnection;
        }

        protected void ChangePage(Page page)
        {
            if (page == null)
            {
                throw new NullReferenceException("Page cannot be null.");
            }

            if (this is SpeechControllable)
            {
                ((SpeechControllable)this).StopSpeechRecognition();
            }

            window.Content = page;

            if (page is SpeechControllable)
            {
                ((SpeechControllable)page).EnableSpeechRecognition();
            }
        }

        protected void MoveBack()
        {
            ChangePage(previousPage);
        }
    }
}
