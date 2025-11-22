using craftingTask.model;
using craftingTask.model.objects;
using craftingTask.model.services;
using craftingTask.persistence;
using craftingTask.persistence.managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace craftingTask.view.frame_pages
{
  /// <summary>
  /// Lógica de interacción para SearchFilterPage.xaml
  /// </summary>
  public partial class SearchFilterPage : Page
  {
    private readonly TaskSearchService searchService;
    private readonly BoardManager boardManager;
    private readonly PanelManager panelManager;
    private readonly StatusManager statusManager;
    private readonly SavedFilterManager filterManager;
    private List<model.objects.Task> searchResults;

    public SearchFilterPage()
    {
      InitializeComponent();
      this.Dispatcher.Invoke(() => this.UpdateLayout(), System.Windows.Threading.DispatcherPriority.Loaded);

      searchService = new TaskSearchService();
      boardManager = new BoardManager();
      panelManager = new PanelManager();
      statusManager = new StatusManager();
      filterManager = new SavedFilterManager();
      searchResults = new List<model.objects.Task>();

      LoadData();
    }

    private void btnCloseSearchFilterFrame(object sender, RoutedEventArgs e)
    {
      Frame? parentFrame = HelpMethods.FindParentFrame(this);
      if (parentFrame != null)
      {
        parentFrame.Content = null;
      }
    }

    private void LoadData()
    {
      // Load priorities
      lstPriorities.Items.Clear();
      lstPriorities.Items.Add(new Priority { Id = 0, Name = "Inmediata" });
      lstPriorities.Items.Add(new Priority { Id = 1, Name = "Alta" });
      lstPriorities.Items.Add(new Priority { Id = 2, Name = "Media" });
      lstPriorities.Items.Add(new Priority { Id = 3, Name = "Baja" });

      lstPriorities.DisplayMemberPath = "Name";
      lstPriorities.SelectedValuePath = "Id";

      // Load statuses
      var statuses = statusManager.GetAllStatus();
      lstStatus.ItemsSource = statuses;
      lstStatus.DisplayMemberPath = "Name";
      lstStatus.SelectedValuePath = "StatusId";

      // Load panels
      var panels = panelManager.GetAllPanels();
      lstPanels.ItemsSource = panels;

      // Load boards
      var boards = boardManager.GetAllBoards();
      lstBoards.ItemsSource = boards;

      // Load saved filters
      LoadSavedFilters();
    }

    private void LoadSavedFilters()
    {
      var filters = filterManager.GetAllFilters();
      cmbSavedFilters.ItemsSource = filters;
      cmbSavedFilters.SelectedIndex = -1;
    }

    private void btnSearch_Click(object sender, RoutedEventArgs e)
    {
      PerformSearch();
    }

    private void PerformSearch()
    {
      var criteria = new SearchCriteria();

      // Keyword
      if (!string.IsNullOrWhiteSpace(txtKeyword.Text))
      {
        criteria.Keyword = txtKeyword.Text;
      }

      // Priorities
      if (lstPriorities.SelectedItems.Count > 0)
      {
        criteria.Priorities = lstPriorities.SelectedItems.Cast<Priority>().Select(x => x.Id).ToList();
      }

      // Statuses
      if (lstStatus.SelectedItems.Count > 0)
      {
        criteria.StatusIds = lstStatus.SelectedItems.Cast<Status>().Select(s => s.StatusId).ToList();
      }

      // Date range
      if (dpStartDate.SelectedDate.HasValue)
      {
        criteria.StartDate = dpStartDate.SelectedDate.Value;
      }
      if (dpEndDate.SelectedDate.HasValue)
      {
        criteria.EndDate = dpEndDate.SelectedDate.Value;
      }

      // Panels
      if (lstPanels.SelectedItems.Count > 0)
      {
        criteria.PanelIds = lstPanels.SelectedItems.Cast<model.objects.Panel>().Select(p => p.PanelId).ToList();
      }

      // Boards
      if (lstBoards.SelectedItems.Count > 0)
      {
        criteria.BoardIds = lstBoards.SelectedItems.Cast<Board>().Select(b => b.BoardId).ToList();
      }

      // Perform search
      searchResults = searchService.Search(criteria);

      // Actualizar ItemsSource en el hilo de UI para evitar errores visuales
      Dispatcher.Invoke(() =>
      {
        lstResults.ItemsSource = null;  // Primero desasignar para limpiar posibles inconsistencias
        lstResults.ItemsSource = searchResults;
        lstResults.InvalidateVisual();  // Forzar invalidación visual
        lstResults.UpdateLayout();      // Forzar actualización de layout
      });
    }

    private void btnClear_Click(object sender, RoutedEventArgs e)
    {
      ClearFilters();
    }

    private void ClearFilters()
    {
      txtKeyword.Clear();
      lstPriorities.SelectedItems.Clear();
      lstStatus.SelectedItems.Clear();
      dpStartDate.SelectedDate = null;
      dpEndDate.SelectedDate = null;
      lstPanels.SelectedItems.Clear();
      lstBoards.SelectedItems.Clear();
      cmbSavedFilters.SelectedIndex = -1;
      lstResults.ItemsSource = null;
      searchResults.Clear();
    }

    private void btnSaveFilter_Click(object sender, RoutedEventArgs e)
    {
      var criteria = new SearchCriteria();

      // Build criteria from current selections
      if (!string.IsNullOrWhiteSpace(txtKeyword.Text))
        criteria.Keyword = txtKeyword.Text;

      if (lstPriorities.SelectedItems.Count > 0)
        criteria.Priorities = lstPriorities.SelectedItems.Cast<Priority>().Select(x => x.Id).ToList();

      if (lstStatus.SelectedItems.Count > 0)
        criteria.StatusIds = lstStatus.SelectedItems.Cast<Status>().Select(s => s.StatusId).ToList();

      if (dpStartDate.SelectedDate.HasValue)
        criteria.StartDate = dpStartDate.SelectedDate.Value;

      if (dpEndDate.SelectedDate.HasValue)
        criteria.EndDate = dpEndDate.SelectedDate.Value;

      if (lstPanels.SelectedItems.Count > 0)
        criteria.PanelIds = lstPanels.SelectedItems.Cast<model.objects.Panel>().Select(p => p.PanelId).ToList();

      if (lstBoards.SelectedItems.Count > 0)
        criteria.BoardIds = lstBoards.SelectedItems.Cast<Board>().Select(b => b.BoardId).ToList();

      if (criteria.IsEmpty)
      {
        MessageBox.Show("No hay criterios de búsqueda para guardar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      // Ask for filter name
      var filterName = Microsoft.VisualBasic.Interaction.InputBox("Ingrese un nombre para el filtro:", "Guardar Filtro", "Mi Filtro");

      if (string.IsNullOrWhiteSpace(filterName))
        return;

      try
      {
        var savedFilter = new SavedFilter(filterName, criteria);
        savedFilter.Add();
        LoadSavedFilters();
        MessageBox.Show("Filtro guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error al guardar el filtro: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void cmbSavedFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cmbSavedFilters.SelectedItem is SavedFilter selectedFilter)
      {
        LoadFilterCriteria(selectedFilter.GetCriteria());
      }
    }

    private void LoadFilterCriteria(SearchCriteria criteria)
    {
      ClearFilters();

      if (criteria.HasKeyword)
        txtKeyword.Text = criteria.Keyword;

      if (criteria.HasPriorities && criteria.Priorities != null)
      {
        foreach (var priority in criteria.Priorities)
        {
          var item = lstPriorities.Items.Cast<Priority>().FirstOrDefault(x => x.Id == priority);
          if (item != null)
            lstPriorities.SelectedItems.Add(item);
        }
      }

      if (criteria.HasStatusFilter && criteria.StatusIds != null)
      {
        var statusList = lstStatus.ItemsSource as List<Status>;
        if (statusList != null)
        {
          foreach (var statusId in criteria.StatusIds)
          {
            var status = statusList.FirstOrDefault(s => s.StatusId == statusId);
            if (status != null)
              lstStatus.SelectedItems.Add(status);
          }
        }
      }

      if (criteria.StartDate.HasValue)
        dpStartDate.SelectedDate = criteria.StartDate.Value;

      if (criteria.EndDate.HasValue)
        dpEndDate.SelectedDate = criteria.EndDate.Value;

      if (criteria.HasPanelFilter && criteria.PanelIds != null)
      {
        var panelList = lstPanels.ItemsSource as List<model.objects.Panel>;
        if (panelList != null)
        {
          foreach (var panelId in criteria.PanelIds)
          {
            var panel = panelList.FirstOrDefault(p => p.PanelId == panelId);
            if (panel != null)
              lstPanels.SelectedItems.Add(panel);
          }
        }
      }

      if (criteria.HasBoardFilter && criteria.BoardIds != null)
      {
        var boardList = lstBoards.ItemsSource as List<Board>;
        if (boardList != null)
        {
          foreach (var boardId in criteria.BoardIds)
          {
            var board = boardList.FirstOrDefault(b => b.BoardId == boardId);
            if (board != null)
              lstBoards.SelectedItems.Add(board);
          }
        }
      }
    }

    private void txtKeyword_TextChanged(object sender, TextChangedEventArgs e)
    {
      // Auto-search on keyword change (optional)
    }

    private void lstResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (lstResults.SelectedItem is model.objects.Task selectedTask)
      {
        MessageBox.Show($"Tarea seleccionada: {selectedTask.Title}\nID: {selectedTask.TaskId}\nPanel ID: {selectedTask.PanelId}",
                        "Información de Tarea", MessageBoxButton.OK, MessageBoxImage.Information);
      }
    }
  }
}
