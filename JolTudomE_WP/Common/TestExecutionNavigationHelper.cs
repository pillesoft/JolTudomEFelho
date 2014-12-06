using JolTudomE_WP.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace JolTudomE_WP.Common {
  public class TestExecutionNavigationHelper:NavigationHelper {
    private TestExecutePage _TEPage;

    public TestExecutionNavigationHelper(Page page) : base(page) {
      _TEPage = (TestExecutePage)page;
    }
    
    public override void GoBack() {
      _TEPage.TestCancel();
    }
  }
}
