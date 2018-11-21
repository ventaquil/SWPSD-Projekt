using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cinema
{
    public class Page : System.Windows.Controls.Page
    {
        protected readonly Page previousPage;

        protected readonly SqlConnection sqlConnection;

        protected readonly Window window;

        public Page() : base()
        {
        }

        public Page(Window window, SqlConnection sqlConnection) : this(window, null, sqlConnection)
        {
        }

        public Page(Window window, Page previousPage, SqlConnection sqlConnection) : this()
        {
            this.window = window;

            this.previousPage = previousPage;

            this.sqlConnection = sqlConnection;
        }

        protected void ChangePage(Page page)
        {
            if (this is ISpeechRecognize)
            {
                ((ISpeechRecognize)this).StopSpeechRecognition();
            }

            window.Content = page ?? throw new NullReferenceException("Page cannot be null.");

            if (page is ISpeechRecognize)
            {
                ((ISpeechRecognize)page).EnableSpeechRecognition();
            }
        }

        protected void MoveBack()
        {
            ChangePage(previousPage);
        }
    }
}
