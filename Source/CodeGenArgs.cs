using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;

namespace NSCs_codegen {
    class CodeGenArgs {

        #region ctor
        internal CodeGenArgs() {
            provider = new CSharpCodeProvider();
            argsToProcess = new List<string>();

            nameSpace = "Colt.Database";
            server = "colt-sql";
            outDir = null;
            database = null;

            opts = new CodeGeneratorOptions();
            opts.BlankLinesBetweenMembers = false;
            opts.ElseOnClosing = true;
            opts.VerbatimOrder = true;

            tables = new List<string>();
        }
        #endregion

        #region properties
        public List<string> argsToProcess { get; private set; }
        public string outDir { get; set; }
        public string nameSpace { get; set; }
        public string database { get; set; }
        public string server { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public List<string> tables { get; private set; }
        public bool showHelp { get; set; }
        public bool generateFields { get; set; }
        public CodeDomProvider provider { get; set; }
        public CodeGeneratorOptions opts { get; set; }
        internal DbProviderFactory providerFactory { get; private set; }
        #endregion

        #region methods
        internal static CodeGenArgs parseArgs(string[] args) {
            CodeGenArgs ret = new CodeGenArgs();
            string anArg;
            int len, nargs;

            nargs = args.Length;
            for (int i = 0; i < nargs; i++) {
                anArg = args[i];
                if ((len = anArg.Length) >= 2) {
                    if (anArg[0] == '-' || anArg[0] == '/') {
                        switch (anArg[1]) {
                            case 'B':ret.provider = new Microsoft.VisualBasic.VBCodeProvider();break;
                            case 'd':
                                if (len > 2)
                                    ret.database = anArg.Substring(2).Trim();
                                else { ret.database = args[i + 1]; i++; }
                                break;
                            case 'f': ret.generateFields = true; break;
                            case 'n':
                                if (len > 2)
                                    ret.nameSpace = anArg.Substring(2).Trim();
                                else { ret.nameSpace = args[i + 1]; i++; }
                                break;
                            case 'o':
                                if (len > 2)
                                    ret.outDir = Path.GetFullPath(anArg.Substring(2).Trim());
                                else { ret.outDir = Path.GetFullPath(args[i + 1]); i++; }
                                break;
                            case 'P':
                                if (len > 2)
                                    ret.password = anArg.Substring(2).Trim();
                                else { ret.password = args[i + 1].Trim(); i++; }
                                break;
                            case 'S':
                                if (len > 2)
                                    ret.server = anArg.Substring(2).Trim();
                                else { ret.server = args[i + 1]; i++; }
                                break;
                            case 'U':
                                if (len > 2)
                                    ret.userName = anArg.Substring(2).Trim();
                                else { ret.userName = args[i + 1].Trim(); i++; }
                                break;
                            case 't':
                                if (len > 2)
                                    ret.tables.Add(anArg.Substring(2).Trim());
                                else { ret.tables.Add(args[i + 1].Trim()); i++; }
                                break;
                            case 'h': ret.showHelp = true; break;
                            case '?': ret.showHelp = true; break;
                            default: Console.Error.WriteLine("Unhandled arg: " + anArg); ret.showHelp = true; break;
                        }
                    } else {
                        ret.argsToProcess.Add(anArg);
                    }
                }
            }
            return ret;
        }

        internal void setProvider(string v) {
            providerFactory = DbProviderFactories.GetFactory(v);
        }

        internal void showHelpText(TextWriter tw) {
            Assembly a;

            a = Assembly.GetEntryAssembly();
            tw.WriteLine(Environment.NewLine+"usage: " + a.GetName().Name +" "+
                "[-B] " +
                "[-f] " +
                "[-h] " +
                "[-?] " +
                "[-d database] " +
                "[-n namespace] " +
                "[-o outdir] " +
                "[-P password] " +
                "[-U username] " +
                "[-S server] " +
                "[-t table  ...[-t table]]");
        }
        #endregion
    }
}