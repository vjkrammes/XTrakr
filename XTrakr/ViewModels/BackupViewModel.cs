using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

using XTrakr.Common;
using XTrakr.Enumerations;
using XTrakr.Infrastructure;
using XTrakr.Repositories.Interfaces;
using XTrakr.Views;

namespace XTrakr.ViewModels;

public class BackupViewModel : ViewModelBase
{
    #region Properties

    private readonly ExplorerViewModel? _explorerViewModel;
    private readonly IDatabase? _database;

    private string? _filename;
    public string? Filename
    {
        get => _filename;
        set => SetProperty(ref _filename, value);
    }

    private string? _directory;
    public string? Directory
    {
        get => _directory;
        set => SetProperty(ref _directory, value);
    }

    private ObservableCollection<FileInfo>? _files;
    public ObservableCollection<FileInfo>? Files
    {
        get => _files;
        set => SetProperty(ref _files, value);
    }

    private FileInfo? _selectedFile;
    public FileInfo? SelectedFile
    {
        get => _selectedFile;
        set => SetProperty(ref _selectedFile, value);
    }

    #endregion

    #region Commands

    private RelayCommand? _changeCommand;
    public ICommand ChangeCommand
    {
        get
        {
            if (_changeCommand is null)
            {
                _changeCommand = new(parm => ChangeClick(), parm => AlwaysCanExecute());
            }
            return _changeCommand;
        }
    }

    private RelayCommand? _backupCommand;
    public ICommand BackupCommand
    {
        get
        {
            if (_backupCommand is null)
            {
                _backupCommand = new(parm => BackupClick(), parm => BackupCanClick());
            }
            return _backupCommand;
        }
    }

    private RelayCommand? _deleteCommand;
    public ICommand DeleteCommand
    {
        get
        {
            if (_deleteCommand is null)
            {
                _deleteCommand = new(parm => DeleteClick(), parm => DeleteCanClick());
            }
            return _deleteCommand;
        }
    }

    #endregion

    #region Command Methods

    private void ChangeClick()
    {
        _explorerViewModel!.IsFolderPicker = true;
        if (DialogSupport.ShowDialog<ExplorerWindow>(_explorerViewModel!, Application.Current.MainWindow) != true)
        {
            return;
        }
        Directory = _explorerViewModel!.SelectedItem!.Path;
        Filename = Directory + @"\" + _database!.Name + ".backup";
        LoadFiles();
    }

    private bool BackupCanClick() => !string.IsNullOrWhiteSpace(Filename);

    private void BackupClick()
    {
        if (string.IsNullOrWhiteSpace(Filename))
        {
            return;
        }
        try
        {
            _database!.BackupDatabase(Filename);
        }
        catch (Exception ex)
        {
            PopupManager.Popup("Database Backup Failed", "Database Error", ex.Innermost(), PopupButtons.Ok, PopupImage.Error);
            return;
        }
        PopupManager.Popup("Database Backup Complete", "Backup Complete", PopupButtons.Ok, PopupImage.Information);
        LoadFiles();
    }

    private bool DeleteCanClick() => SelectedFile is not null;

    private void DeleteClick()
    {
        if (SelectedFile is null)
        {
            return;
        }
        var msg = $"Delete backup file '{SelectedFile.FullName}'? Action cannot be undone.";
        if (PopupManager.Popup("Delete Backup File?", "Delete File?", msg, PopupButtons.YesNo, PopupImage.Question) != PopupResult.Yes)
        {
            SelectedFile = null;
            return;
        }
        try
        {
            File.Delete(SelectedFile.FullName);
        }
        catch (Exception ex)
        {
            PopupManager.Popup("Delete of File Failed", "I/O Error", ex.Innermost(), PopupButtons.Ok, PopupImage.Error);
            SelectedFile = null;
            return;
        }
        LoadFiles();
    }

    #endregion

    #region Utility Methods

    private void LoadFiles()
    {
        if (string.IsNullOrWhiteSpace(Directory))
        {
            return;
        }
        Files = new ObservableCollection<FileInfo>();
        try
        {
            var files = System.IO.Directory.GetFiles(Directory, "*.backup");
            foreach (var file in files)
            {
                Files.Add(new FileInfo(file));
            }
        }
        catch
        {
            Directory = string.Empty;
        }
    }

    #endregion

    public BackupViewModel(IDatabase database, ExplorerViewModel explorerViewModel)
    {
        _database = database;
        _explorerViewModel = explorerViewModel;
        Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }
}
