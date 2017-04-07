using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.CSharp;

namespace NSCs_codegen {

    class CodeGenArgs {

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
        }

        public List<string> argsToProcess { get; private set; }
        public string outDir { get;   set; }
        public string nameSpace { get; set; }
        public string database { get; set; }
        public string server { get; set; }
        public bool showHelp { get; set; }
        public CodeDomProvider provider { get;  set; }
        public CodeGeneratorOptions opts { get;  set; }
        internal  static CodeGenArgs parseArgs(string[] args) {
            CodeGenArgs ret = new CodeGenArgs();
            string anArg;
            int len,nargs;

            nargs = args.Length;
            for (int i = 0; i < nargs; i++) {
                anArg = args[i];
                if ((len = anArg.Length) >= 2) {
                    if (anArg[0] == '-' || anArg[0] == '/') {
                        switch (anArg[1]) {
                            case 'd':
                                if (len > 2)
                                    ret.database = anArg.Substring(2).Trim();
                                else { ret.database = args[i + 1]; i++; }
                                break;
                            case 'n':
                                if (len > 2)
                                    ret.nameSpace = anArg.Substring(2).Trim();
                                else { ret.nameSpace = args[i + 1]; i++; }
                                break;
                            case 'o':
                                if (len > 2)
                                    ret.outDir = anArg.Substring(2).Trim();
                                else { ret.outDir = args[i + 1]; i++; }
                                break;
                            case 's':
                                if (len > 2)
                                    ret.server = anArg.Substring(2).Trim();
                                else { ret.server = args[i + 1]; i++; }
                                break;
                            case 'h': ret.showHelp = true; break;
                            case '?': ret.showHelp = true; break;
                        }
                    } else {
                      ret.  argsToProcess.Add(anArg);
                    }
                }
            }
            return ret;
        }

        internal void setProvider(string v) {
            providerFactory = DbProviderFactories.GetFactory(v);
        }
        internal DbProviderFactory providerFactory { get; private set; }
    }
}