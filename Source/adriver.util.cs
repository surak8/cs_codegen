using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NSCs_codegen {
    public partial class cs_codegenDriver {

        #region fields
        static readonly CodeStatement csBlank = new CodeSnippetStatement();
        static readonly CodeStatement csBreak = new CodeSnippetStatement("\t\t\tbreak;");

        static readonly CodeExpression ceThis = new CodeThisReferenceExpression();
        static readonly CodeExpression ceValue = new CodePropertySetValueReferenceExpression();
        static readonly CodeExpression ceTrue = new CodePrimitiveExpression(true);
        static readonly CodeExpression ceFalse = new CodePrimitiveExpression(false);
        static readonly CodeExpression ceZero = new CodePrimitiveExpression(0);
        static readonly CodeExpression ceOne = new CodePrimitiveExpression(1);

        public static bool showCode = false;
        static IDictionary<string, string> nameMap = new Dictionary<string, string>();
        #endregion

//<<<<<<< Updated upstream
//        static void generateCodeFromProcedure(IDbConnection conn, string procName, string outputPath, string nameSpace, CodeDomProvider cdp, CodeGeneratorOptions opts) {
//            SqlDataReader reader;
//=======
        static void generateCodeFromProcedure(IDbConnection conn, string procName, CodeGenArgs args) {
        SqlDataReader reader;
//>>>>>>> Stashed changes

            try {
                using (SqlCommand cmd = new SqlCommand(procName, (SqlConnection) conn)) {
                    reader = cmd.ExecuteReader();
//<<<<<<< Updated upstream
//                    generateStuff(makeClassName(procName), nameSpace, outputPath, cdp, opts, reader, procName);
//=======
//<<<<<<< Updated upstream
//                    generateStuff(makeClassName(procName), nameSpace, outputPath, cdp, opts, reader,procName);
//=======
                    generateStuff(makeClassName(procName), args, reader,procName);
//>>>>>>> Stashed changes
//>>>>>>> Stashed changes
                    reader.Close();
                }
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message);
            }
        }

//<<<<<<< Updated upstream
//        static void generateStuff(string className, string outDir, string nameSpace, CodeDomProvider cdp, CodeGeneratorOptions opts, SqlDataReader reader, string tableName) {
//=======
//<<<<<<< Updated upstream
//        static void generateStuff(string className, string outDir, string nameSpace, CodeDomProvider cdp, CodeGeneratorOptions opts, SqlDataReader reader,string tableName) {
//=======
        static void generateStuff(string className, CodeGenArgs args, IDataReader  reader,string tableName) {
//>>>>>>> Stashed changes
//>>>>>>> Stashed changes
            CodeCompileUnit ccu;
            CodeNamespace ns, ns0;
            CodeTypeDeclaration ctd;
            string tmp, fldName, propName;
            Type colType;

//<<<<<<< Updated upstream
//=======
            CodeDomProvider cdp = args.provider;
#warning check this. 
            string nameSpace = args.nameSpace;
//            CodeMemberProperty p;
//>>>>>>> Stashed changes
            CodeStatementCollection csc, csc2;

            CodeVariableReferenceExpression vr, vrI;
            CodeArgumentReferenceExpression ar;
            CodeConstructor cc;
            CodeStatement csCN;
            CodeStatement csswitch;
            CodeIterationStatement cis;
#if FIELD_AND_PROPERTY
            CodeMemberProperty p;
            CodeMemberField f;
            CodeMemberProperty firstProp = null;
            CodeMemberField firstField = null;

            p = null;
            f = null;
#endif
//<<<<<<< Updated upstream
//            if (string.IsNullOrEmpty(outDir))
//                outDir = Directory.GetCurrentDirectory();

//            if (!Directory.Exists(outDir))
//                Directory.CreateDirectory(outDir);

//=======
              if (string.IsNullOrEmpty(args.outDir))
                    args.outDir = Directory.GetCurrentDirectory();

                if (!Directory.Exists(args.outDir))
                    Directory.CreateDirectory(args.outDir);
//>>>>>>> Stashed changes

            //    foreach()
            ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(ns = ns0 = new CodeNamespace());
            ns0.Imports.Add(new CodeNamespaceImport("System"));
            ns0.Imports.Add(new CodeNamespaceImport("System.Data"));
            if (!string.IsNullOrEmpty(nameSpace))
                ccu.Namespaces.Add(ns = new CodeNamespace(nameSpace));
            ns.Types.Add(ctd = new CodeTypeDeclaration(className));
            ctd.BaseTypes.Add(new CodeTypeReference("ColtBaseRecord"));
            ctd.IsPartial = true;

//<<<<<<< Updated upstream
            addTablenameConstant(tableName, ctd);

            //generateCSVFrom(reader, tableName, outDir, ctd, ns0);
            //=======
            generateCSVFrom(reader, tableName, args.outDir, ctd,ns0);
            //>>>>>>> Stashed changes

            ar = new CodeArgumentReferenceExpression("reader");
            cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;
            cc.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "ctor"));
            cc.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "ctor"));
            vr = new CodeVariableReferenceExpression("colName");
            vrI = new CodeVariableReferenceExpression("i");
            csc = new CodeStatementCollection();
#if true
            csCN = new CodeAssignStatement(vr,
                new CodeMethodInvokeExpression(ar, "GetName", vrI));
#else
            csCN = new CodeExpressionStatement(
                new CodeBinaryOperatorExpression(vr, CodeBinaryOperatorType.Assign,
                new CodeMethodInvokeExpression(ar, "GetName", vrI)));
//                ))
#endif

            csc2 = new CodeStatementCollection();
            csswitch = new CodeSnippetStatement("\t\t\tswitch(" + removeTrailingChar(generate(csCN, cdp, args.opts), ';') + ") {");
            csc2.Add(csswitch);

            tmp = removeParensFrom(generate(new CodeExpressionStatement(
                                new CodeBinaryOperatorExpression(vrI,
                                    CodeBinaryOperatorType.Assign, new CodeBinaryOperatorExpression(vrI, CodeBinaryOperatorType.Add, ceOne))), cdp, args.opts));
            csc.AddRange(new CodeStatement[] {
                        new CodeVariableDeclarationStatement(typeof(string),vr.VariableName),
                        csBlank,
                        cis=new CodeIterationStatement(
                            new CodeVariableDeclarationStatement(typeof(int),vrI.VariableName,ceZero),
                            new CodeBinaryOperatorExpression (vrI,  CodeBinaryOperatorType.LessThan ,
                            new CodePropertyReferenceExpression (ar,"FieldCount")),
                            new CodeSnippetStatement (tmp))

                    });
            for (int i = 0; i < reader.FieldCount; i++) {
                tmp = reader.GetName(i);
                fldName = makeFieldName(tmp);
                propName = makePropName(tmp);
                colType = reader.GetFieldType(i);
#if FIELD_AND_PROPERTY
                ctd.Members.AddRange(
                    new CodeTypeMember[] {
                        f=makeField(fldName ,colType),
                        p=makeProperty(propName,f,i)
                    });
                if (firstProp == null)
                    firstProp = p;
                if (firstField == null)
                    firstField = f;
#else
#if true
                ctd.Members.Add(new CodeSnippetTypeMember("\t\tpublic " + cdp.GetTypeOutput(new CodeTypeReference(colType)) + " " + propName + " { get; set; }\r\n"));
#else
                ctd.Members.Add(p = new CodeMemberProperty());
                p.Name = propName;
                p.Type = new CodeTypeReference(colType);
                p.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                p.HasGet = true;
                p.HasSet = true;
#endif
#endif
                addCaseStatementsTo(csc2, tmp,
                    new CodePropertyReferenceExpression(ceThis, propName),
                    colType, ar, vrI, cdp, args.opts);
            }

#if FIELD_AND_PROPERTY
            firstProp.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "properties"));
            p.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "properties"));

            firstField.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "fields"));
            f.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "fields"));
#endif

            csc2.Add(new CodeSnippetStatement("\t\t}"));
            cis.Statements.AddRange(csc2);
            ctd.Members.Add(cc);
            cc.Statements.AddRange(csc);
            cc.Parameters.Add(new CodeParameterDeclarationExpression("IDataReader", ar.ParameterName));

            addColtStuff(ctd);
            if (ccu != null) {
                StringBuilder sb;

                #warning check this patch
//<<<<<<< Updated upstream
//                string fname = Path.Combine(outDir, className + "." + cdp.FileExtension);
//                using (StringWriter sw = new StringWriter(sb = new StringBuilder())) {
//                    cdp.GenerateCodeFromCompileUnit(ccu, sw, opts);
//=======
//<<<<<<< Updated upstream
  //           StringBuilder sb;
//                string fname = Path.Combine(outDir, className + "." + cdp.FileExtension);
//=======
                if (string.IsNullOrEmpty(args.outDir))
                    args.outDir = Directory.GetCurrentDirectory();

                if (!Directory.Exists(args.outDir))
                    Directory.CreateDirectory(args.outDir);

                //StringBuilder sb;
                string fname = Path.Combine(args.outDir, className + "." + cdp.FileExtension);
//>>>>>>> Stashed changes
                using (StringWriter sw = new StringWriter(sb = new StringBuilder())) {
                    //                    Trace.WriteLine("here");
                    cdp.GenerateCodeFromCompileUnit(ccu, sw, args.opts);
//>>>>>>> Stashed changes
                }
                if (showCode)
                    Trace.WriteLine("Code is:" + Environment.NewLine + sb.ToString());
                File.WriteAllText(fname, sb.ToString());
                Trace.WriteLine("wrote: " + fname);
                Console.WriteLine("wrote: " + fname);
            }
        }

          static void addColtStuff(CodeTypeDeclaration ctd) {
            CodeMemberMethod m;

            ctd.Members.Add(m = new CodeMemberMethod());
            //            m.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            m.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            m.Name = "isModified";
            m.ReturnType = new CodeTypeReference(typeof(bool));
            m.Statements.Add(new CodeMethodReturnStatement(ceFalse));
        }

        static void addTablenameConstant(string tableName, CodeTypeDeclaration ctd) {
            CodeTypeMember ctm = addClassConstant(ctd, "TABLE_NAME", tableName);

            addRegion("constants", ctm);
        }

        static void addRegion(string regionName, CodeTypeMember ctm) {
            addRegion(regionName, ctm, ctm);
        }

        static void addRegion(string regionName, CodeTypeMember ctmStart, CodeTypeMember ctmEnd) {
            ctmStart.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, regionName));
            ctmEnd.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, regionName));
        }

        static CodeTypeMember addClassConstant(CodeTypeDeclaration ctd, string constantName, string constantValue) {
            CodeMemberField ftabname = new CodeMemberField(typeof(string), constantName);

            ftabname.InitExpression = new CodePrimitiveExpression(constantValue);
            ftabname.Attributes = MemberAttributes.Const | MemberAttributes.Public;
            ctd.Members.Add(ftabname);
            return ftabname;
        }

        static void generateCSVFrom(IDataReader reader, string tableName, string outDir, CodeTypeDeclaration ctd, CodeNamespace ns) {
//=======
//        static void generateCSVFrom(IDataReader reader, string tableName, string outDir, CodeTypeDeclaration ctd) {
//>>>>>>> Stashed changes
            DataTable dt = reader.GetSchemaTable();

            //            writeCSV(constantValue, outDir, dt);
            generateColumnCollection(ctd, ns, dt);
        }

        static void writeCSV(string tableName, string outDir, DataTable dt) {
            int i0 = 0;
            string filename;

            using (TextWriter tw = new StreamWriter(filename = Path.Combine(outDir, tableName + ".csv"))) {
                foreach (DataColumn dc in dt.Columns) {
                    if (i0 > 0)
                        tw.Write(",");
                    tw.Write(dc.ColumnName);
                    i0++;
                }
                tw.WriteLine();

                foreach (DataRow dr in dt.Rows) {
                    i0 = 0;
                    foreach (DataColumn dc in dt.Columns) {
                        if (i0 > 0)
                            tw.Write(",");
                        tw.Write(dr[dc]);
                        i0++;
                    }
                    tw.WriteLine();
                }
                tw.WriteLine();
            }
            Debug.Print("generated: " + filename);
        }

        const string MOD_VECTOR_NAME = "_modified";

        static void generateColumnCollection(CodeTypeDeclaration ctd, CodeNamespace ns, DataTable dt) {
            CodeTypeReference ctrColDef = new CodeTypeReference("ColumnDef");
            CodeExpression[] args = makeClassFieldCollection(dt, ctrColDef);
            CodeMemberField f, f3;
            CodeMemberProperty f2;

            ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            ctd.Members.Add(f = new CodeMemberField());
            f.Attributes = MemberAttributes.Static;
            f.Name = "_fields";
            f.Type = new CodeTypeReference("List", ctrColDef);
            f.InitExpression = new CodeObjectCreateExpression(f.Type, new CodeArrayCreateExpression(ctrColDef, args));

            f3 = new CodeMemberField(new CodeTypeReference(new CodeTypeReference(typeof(bool)), 1),MOD_VECTOR_NAME);
            f3.Attributes = 0;
            f3.InitExpression = new CodeArrayCreateExpression(f3.Type, new CodePrimitiveExpression(args.Length));
            ctd.Members.Add(f3);
            addRegion("fields", f, f3);

            ctd.Members.Add(f2 = new CodeMemberProperty());
            f2.Name = "fields";
            f2.Attributes = MemberAttributes.Static | MemberAttributes.Public;
            f2.Type = f.Type;
            f2.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, f.Name)));
            addRegion("properties", f2);
        }

        static CodeExpression[] makeClassFieldCollection(DataTable dt, CodeTypeReference ctrColDef) {
            List<CodeExpression> exprs = new List<CodeExpression>();
            /*
             * ColumnName,
             * ColumnOrdinal,
             * ColumnSize,
             * IsUnique? (not working)
             * DataType,
             * AllowDBNull,
             * DataTypeName
             * */
            CodeObjectCreateExpression ret = new CodeObjectCreateExpression(ctrColDef);
            foreach (DataRow dr in dt.Rows) {
                exprs.Add(ret = new CodeObjectCreateExpression(ctrColDef));
                ret.Parameters.AddRange(new CodeExpression[] {
                    new CodePrimitiveExpression(dr["ColumnName"].ToString()),
                    new CodePrimitiveExpression (dr["DataTypeName"].ToString()),
                    new CodeTypeOfExpression (dr["DataType"].ToString()),
                    new CodePrimitiveExpression(Convert.ToInt32 (dr["ColumnOrdinal"])),
                    new CodePrimitiveExpression(Convert.ToInt32 (dr["ColumnSize"])),
                    new CodePrimitiveExpression(Convert.ToBoolean(dr["AllowDBNull"])),
                    new CodePrimitiveExpression(Convert.ToBoolean(dr["IsUnique"]))
                });
                //               Debug.Print("found " + dr["IsKey"]);
            }
            return exprs.ToArray();
        }

        static CodeExpression createColDef(CodeTypeReference ctr, string fldName, string fldValue) {
            return new CodeObjectCreateExpression(
                ctr,
                new CodeBinaryOperatorExpression(
                    new CodeSnippetExpression(fldName),
                    CodeBinaryOperatorType.Assign,
                    new CodeSnippetExpression("\"" + fldValue + "\"")));
        }
        static CodeExpression createColDef(CodeTypeReference ctr, string fldName, int fldValue) {
            return new CodeObjectCreateExpression(
                ctr,
                new CodeBinaryOperatorExpression(
                    new CodeSnippetExpression(fldName),
                    CodeBinaryOperatorType.Assign,
                    new CodePrimitiveExpression(fldValue)));
        }
        static CodeExpression createColDef(CodeTypeReference ctr, string fldName, bool fldValue) {
            return new CodeObjectCreateExpression(
                ctr,
                new CodeBinaryOperatorExpression(
                    new CodeSnippetExpression(fldName),
                    CodeBinaryOperatorType.Assign,
                    new CodePrimitiveExpression(fldValue)));
        }

        static CodeTypeReference makeTypeReference(Type t, CodeDomProvider cdp) {
            string tmp = cdp.GetTypeOutput(new CodeTypeReference(t));

            return new CodeTypeReference("dummy");
        }

        static string removeParensFrom(string v) {
            string tmp2;
            StringBuilder sb = new StringBuilder();
            string[] blah = v.Split('\r', '\n', '(', ')', ';');

            foreach (string str in blah)
                if (!string.IsNullOrEmpty(tmp2 = str.Trim()))
                    sb.Append(tmp2);
            return sb.ToString();
        }

        static string removeTrailingChar(string v1, char v2) {
            StringBuilder sb;
            string tmp2;
            string[] blah;

            if (!string.IsNullOrEmpty(v1) && v2 != char.MinValue) {
                sb = new StringBuilder();
                blah = v1.Split('\r', '\n', v2);
                foreach (string str in blah)
                    if (!string.IsNullOrEmpty(tmp2 = str.Trim()))
                        sb.Append(tmp2);
                return sb.ToString();
            }
            return v1;
        }

        static void addCaseStatementsTo(CodeStatementCollection csc, string fieldName, CodeExpression propRef, Type colType,
            CodeArgumentReferenceExpression ar, CodeVariableReferenceExpression vrI, CodeDomProvider cdp, CodeGeneratorOptions opts) {

            csc.AddRange(new CodeStatement[] {
                new CodeSnippetStatement("\t\tcase \"" + fieldName + "\":"),
                new CodeAssignStatement(propRef, makeNullAssignment(colType, cdp)),
                new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeMethodInvokeExpression(ar, "IsDBNull", vrI), CodeBinaryOperatorType.IdentityInequality, ceTrue),
                        new CodeCommentStatement("do real assign here"),
                        new CodeAssignStatement(propRef, new CodeMethodInvokeExpression(ar, findTypeMethod(colType, cdp), vrI))),
                csBreak
            });
        }

        static CodeExpression makeNullAssignment(Type colType, CodeDomProvider cdp) {
            string outType = cdp.GetTypeOutput(new CodeTypeReference(colType));

            if (string.Compare(outType, "string", true) == 0) return new CodeSnippetExpression(outType + ".Empty");
            else if (string.Compare(outType, "int", true) == 0) return new CodeSnippetExpression("int.MinValue");
            else if (string.Compare(outType, "long", true) == 0) return new CodeSnippetExpression("long.MinValue");
            else if (string.Compare(outType, "bool", true) == 0) return new CodeSnippetExpression("false");
            else if (string.Compare(outType, "decimal", true) == 0) return new CodeSnippetExpression("Decimal.MinValue");
            else if (string.Compare(outType, "System.Guid", true) == 0) return new CodeSnippetExpression("Guid.Empty");
            else if (string.Compare(outType, "System.DateTime", true) == 0) return new CodeSnippetExpression("DateTime.MinValue");
            throw new InvalidOperationException("Unhandled type: " + colType.FullName);
        }

        static string findTypeMethod(Type colType, CodeDomProvider cdp) {
            string outType = cdp.GetTypeOutput(new CodeTypeReference(colType));

            if (string.Compare(outType, "string", true) == 0) return "GetString";
            else if (string.Compare(outType, "int", true) == 0) return "GetInt32";
            else if (string.Compare(outType, "long", true) == 0) return "GetInt64";
            else if (string.Compare(outType, "bool", true) == 0) return "GetBoolean";
            else if (string.Compare(outType, "decimal", true) == 0) return "GetDecimal";
            else if (string.Compare(outType, "System.Guid", true) == 0) return "GetGuid";
            else if (string.Compare(outType, "System.DateTime", true) == 0) return "GetDateTime";
            throw new InvalidOperationException("Unhandled type: " + colType.FullName);
        }

        static string generate(CodeStatement cs, CodeDomProvider cdp, CodeGeneratorOptions opts) {
            StringBuilder sb = new StringBuilder();

            using (StringWriter sw = new StringWriter(sb)) {
                cdp.GenerateCodeFromStatement(cs, sw, opts);
            }
            return sb.ToString();
        }

        static readonly CodeFieldReferenceExpression frModVector = new CodeFieldReferenceExpression(ceThis, MOD_VECTOR_NAME);

        static CodeMemberProperty makeProperty(string propName, CodeMemberField f,int modVectorIndex) {
            CodeMemberProperty p = new CodeMemberProperty();
            CodeFieldReferenceExpression fr = new CodeFieldReferenceExpression(ceThis, f.Name);

            p.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            p.Name = propName;
            p.Type = f.Type;

            p.SetStatements.Add(new CodeAssignStatement(fr, ceValue));
            if (modVectorIndex >= 0)
                p.SetStatements.Add(
                    new CodeAssignStatement(
                        new CodeArrayIndexerExpression(frModVector, new CodePrimitiveExpression(modVectorIndex)), 
                        ceTrue));
            p.GetStatements.Add(new CodeMethodReturnStatement(fr));
            return p;
        }

        static CodeMemberField makeField(string fldName, Type colType) {
            CodeMemberField f = new CodeMemberField(makeTypeRef(colType), fldName);

            f.Attributes = 0;
            return f;
        }

        static CodeTypeReference makeTypeRef(Type colType) {
            return new CodeTypeReference(colType.FullName);
        }

        static string makePropName(string tmp) {
            return fixup(tmp.Replace(' ', '_'));
        }

        static string makeFieldName(string tmp) {
            return "_" + fixup(tmp.Replace(' ', '_'));
        }

        static string fixup(string procName) {
            return fixup(procName, false);
        }
        static string fixup(string procName, bool isClass) {
            StringBuilder sb;
            int len;
            bool bupper;
            char c;

            if (string.IsNullOrEmpty(procName))
                throw new ArgumentNullException("procName", "string is null!");
            if (!nameMap.ContainsKey(procName)) {
                sb = new StringBuilder();
                len = procName.Length;
                if (isClass)
                    sb.Append(char.ToUpper(procName[0]), 1);
                else
                    sb.Append(char.ToLower(procName[0]), 1);
                bupper = false;
                for (int i = 1; i < len; i++)
                    if ((c = procName[i]) == '_')
                        bupper = true;
                    else {
                        if (char.IsWhiteSpace(c))
                            sb.Append('_', 1);
                        else {
                            if (bupper) {
                                bupper = false;
                                sb.Append(char.ToUpper(c), 1);
                            } else
                                sb.Append(char.ToLower(c), 1);
                        }
                    }
                nameMap.Add(procName, sb.ToString());
            }
            return nameMap[procName];
        }

        static string makeClassName(string procName) {
#if true
            return fixup(procName, true);
#else
            return procName.Substring(0, 1).ToUpper() +
                procName.Substring(1).ToLower();
#endif
        }
    }
}