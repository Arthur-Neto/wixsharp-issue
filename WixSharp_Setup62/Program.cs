using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp_Setup62.Forms;

namespace WixSharp_Setup62
{
    internal class Program
    {
        public static readonly CultureInfo DefaultLanguage = new CultureInfo("en-US");
        public static readonly IList<CultureInfo> SupportedLanguages = new List<CultureInfo>
        {
            new CultureInfo("pt-BR"),
        };

        private static void Main()
        {
            var project = new ManagedProject(
                "XYZ",
                new Dir(@"%ProgramFiles%\Teste\",
                    new File("Program.cs")
                )
            )
            {
                GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b"),
                LocalizationFile = $@"Languages\{DefaultLanguage.Name}.wxl",

                BannerImage = @"Resources\dialog.png",
                BackgroundImage = @"Resources\dialog.png",
                LicenceFile = @"Resources\license.rtf",

                ManagedUI = new ManagedUI(),
            };

            project.UIInitialized += (SetupEventArgs e) =>
            {
                CheckElevatedRights(e);
            };

            project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                            .Add<InstallDirDialog>()
                                            .Add<ProgressDialog>()
                                            .Add<ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
                                           .Add<ProgressDialog>()
                                           .Add<ExitDialog>();

            Localize(project);

            project.Language = DefaultLanguage.Name;
            var productMsi = project.BuildMsi();

            var sb = new StringBuilder();
            sb.Append(DefaultLanguage.Name);
            foreach (var supportedLanguage in SupportedLanguages)
            {
                project.Language = supportedLanguage.Name;
                var mstFile = project.BuildLanguageTransform(productMsi, supportedLanguage.Name, $@"Languages\{supportedLanguage.Name}.wxl");

                productMsi.EmbedTransform(mstFile);

                sb.Append($",{supportedLanguage.Name}");
            }

            productMsi.SetPackageLanguages(sb.ToString().ToLcidList());
        }

        private static void Localize(ManagedProject project)
        {
            project.AddBinary(new Binary(new Id($"{DefaultLanguage.Parent}_xsl"), $@"Languages\{DefaultLanguage.Name}.wxl"));

            foreach (var supportedLanguage in SupportedLanguages)
            {
                project.AddBinary(new Binary(new Id($"{supportedLanguage.Parent}_xsl"), $@"Languages\{supportedLanguage.Name}.wxl"));
            }

            project.UIInitialized += (SetupEventArgs e) =>
            {
                var runtime = e.ManagedUI.Shell.MsiRuntime();

                var languageSelected = SelectLanguage().Name;
                if (languageSelected.Equals(DefaultLanguage.Name))
                {
                    runtime.UIText.InitFromWxl(e.Session.ReadBinary($"{DefaultLanguage.Parent}_xsl"));
                    return;
                }

                foreach (var supportedLanguage in SupportedLanguages)
                {
                    if (supportedLanguage.Name.Equals(languageSelected))
                    {
                        runtime.UIText.InitFromWxl(e.Session.ReadBinary($"{supportedLanguage.Parent}_xsl"));
                    }
                }
            };
        }

        private static CultureInfo SelectLanguage()
        {
            var selectLanguageForm = new SelectLanguageForm();

            selectLanguageForm.ShowDialog();

            return selectLanguageForm.SelectedCulture;
        }

        private static void CheckElevatedRights(SetupEventArgs setupEventArgs)
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                setupEventArgs.Result = ActionResult.Failure;

                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = "msiexec.exe",
                    Arguments = "/i \"" + setupEventArgs.MsiFile + "\"",
                    Verb = "runas"
                };

                Process.Start(startInfo);
            }
        }
    }
}