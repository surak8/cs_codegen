using System;
using System.CodeDom.Compiler;
using System.Data.Common;
using Microsoft.CSharp;

namespace NSCs_codegen {

    class CodeGenArgs {

        internal CodeGenArgs() {
            provider = new CSharpCodeProvider();
            opts = new CodeGeneratorOptions();
            opts.BlankLinesBetweenMembers = false;
            opts.ElseOnClosing = true;
        }

        public string outDir { get;   set; }
        public string nameSpace { get;   set; }
        public CodeDomProvider provider { get;  set; }
        public CodeGeneratorOptions opts { get;  set; }
        internal static CodeGenArgs parseArgs(string[] args) {
            CodeGenArgs ret = new CodeGenArgs();

            return ret;
        }

        internal void setProvider(string v) {
            providerFactory = DbProviderFactories.GetFactory(v);
        }
        internal DbProviderFactory providerFactory { get; private set; }
    }
}