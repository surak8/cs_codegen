using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using NSMisc;

namespace NSCs_codegen {
    public partial class cs_codegenDriver {
        #region constants
        const string FR_MOD_VECTOR_NAME = "_modified";
        #endregion

        #region fields
        static readonly CodeStatement csBlank = new CodeSnippetStatement();
        static readonly CodeStatement csBreak = new CodeSnippetStatement("\t\t\tbreak;");

        static readonly CodeExpression ceThis = new CodeThisReferenceExpression();
        static readonly CodeExpression ceBase = new CodeBaseReferenceExpression();
        static readonly CodeExpression ceValue = new CodePropertySetValueReferenceExpression();
        static readonly CodeExpression ceTrue = new CodePrimitiveExpression(true);
        static readonly CodeExpression ceFalse = new CodePrimitiveExpression(false);
        static readonly CodeExpression ceZero = new CodePrimitiveExpression(0);
        static readonly CodeExpression ceOne = new CodePrimitiveExpression(1);
        static readonly CodeFieldReferenceExpression frModVector = new CodeFieldReferenceExpression(ceThis, FR_MOD_VECTOR_NAME);

        public static bool showCode = false;
        static readonly IDictionary<string, string> nameMap = new Dictionary<string, string>();
        #endregion

        static void generateCodeFromProcedure(DbConnection conn, string procName, CodeGenArgs args) {
#if true

            try {
                using (DbCommand cmd = args.providerFactory.CreateCommand()) {
                    cmd.CommandText = procName;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
#if true
                    IDataReader reader = cmd.ExecuteReader();

                    generateStuff(makeClassName(procName), args, reader, procName);
                    reader.Close();
#else
                    if (cmd.ExecuteNonQuery() > 0)
                        Trace.WriteLine("success");
                    else
                        Trace.WriteLine("failure");
#endif
                }
            } catch (DbException dbex) {
                Logger.log(MethodBase.GetCurrentMethod(), dbex);
            } catch (Exception ex) {
                Logger.log(MethodBase.GetCurrentMethod(), ex);
            } finally {
            }
#else
            SqlDataReader reader;

            try {
                using (SqlCommand cmd = new SqlCommand(procName, (SqlConnection) conn)) {
                    reader = cmd.ExecuteReader();
                    generateStuff(makeClassName(procName), args, reader, procName);
                    reader.Close();
                }
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message);
            }
#endif
        }

        static void generateStuff(string className, CodeGenArgs args, IDataReader reader, string tableName) {
            CodeCompileUnit ccu;
            CodeNamespace ns, ns0;
            CodeTypeDeclaration ctd;
            StringBuilder sb;
            CodeDomProvider cdp = args.provider;
            string nameSpace = args.nameSpace;
#if FIELD_AND_PROPERTY
            CodeMemberProperty p;
            CodeMemberField f;
            CodeMemberProperty firstProp = null;
            CodeMemberField firstField = null;

            p = null;
            f = null;
#endif
            if (string.IsNullOrEmpty(args.outDir))
                args.outDir = Directory.GetCurrentDirectory();

            if (!Directory.Exists(args.outDir))
                Directory.CreateDirectory(args.outDir);

            ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(ns = ns0 = new CodeNamespace());
            ns0.Imports.AddRange(
                new CodeNamespaceImport[] {
                    new CodeNamespaceImport("System"),
                    new CodeNamespaceImport("System.Data"),
                    new CodeNamespaceImport("Colt.Database")
                });

            if (!string.IsNullOrEmpty(nameSpace))
                ccu.Namespaces.Add(ns = new CodeNamespace(nameSpace));
            ns.Types.Add(ctd = new CodeTypeDeclaration(className));
            ctd.BaseTypes.Add(new CodeTypeReference("ColtBaseRecord"));
            if (args.generateFields)
                implementPropertyChanged(ns0, ctd);
            ctd.IsPartial = true;
            addTablenameConstant(tableName, ctd);
            generateCSVFrom(reader, tableName, args.outDir, ctd, ns0,cdp);
            ctd.Members.Add(createDefaultConstructor());
            ctd.Members.Add(addOtherConstructor(args, reader, ctd, cdp));
            ctd.Members.AddRange(addColtStuff(args, ctd, ns0,cdp));
            ctd.Members.AddRange(addAbstractItems(new CodeTypeReference (ctd.Name ),args.provider));

            if (ccu != null) {

                foreach(CodeTypeMember ctm in ctd.Members ) {
                    Trace.WriteLine(ctm.GetType().FullName + ":" + ctm.Name);
                }
                if (string.IsNullOrEmpty(args.outDir))
                    args.outDir = Directory.GetCurrentDirectory();

                if (!Directory.Exists(args.outDir))
                    Directory.CreateDirectory(args.outDir);

                string fname = Path.Combine(args.outDir, className + "." + cdp.FileExtension);
                using (StringWriter sw = new StringWriter(sb = new StringBuilder())) {
                    cdp.GenerateCodeFromCompileUnit(ccu, sw, args.opts);
                }
                if (showCode)
                    Trace.WriteLine("Code is:" + Environment.NewLine + sb.ToString());
                File.WriteAllText(fname, sb.ToString());
                Trace.WriteLine("wrote: " + fname);
                Console.WriteLine("wrote: " + fname);
            }
        }

        static CodeTypeMemberCollection addAbstractItems(CodeTypeReference ctr,CodeDomProvider cdp) {
            CodeTypeMemberCollection ret = new CodeTypeMemberCollection();
            CodeMemberMethod m;

            ret.AddRange(
                new CodeTypeMember[] {
                    addFieldList(
                        new CodeTypeReference("List", new CodeTypeReference("ColumnDef")),
                        "fieldList",
                        new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(ctr), FNAME_FIELDS)),
                    m=addSetModified("setModified",frModVector),
                    addResetModified("resetModifyFlags",m.Name,frModVector,cdp)
                });
            return ret;
        }

        static CodeTypeMember addResetModified(string v, string name, CodeFieldReferenceExpression fre, CodeDomProvider cdp) {
            // protected override void resetModifyFlags() {
            //      for (int i = _modified.GetLowerBound(0); i < _modified.GetUpperBound(0); i++)
            //          setModified(i, false);
            //  }

            CodeMemberMethod m = new CodeMemberMethod();
            CodeVariableReferenceExpression vri = new CodeVariableReferenceExpression("i");
            CodeStatement css;

            css = new CodeSnippetStatement(vri.VariableName + "++");
            if (cdp is Microsoft.VisualBasic.VBCodeProvider)
                css = new CodeAssignStatement(vri, new CodeBinaryOperatorExpression(vri, CodeBinaryOperatorType.Add, ceOne));
            m.Name = v;
            m.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            m.Statements.Add(
                new CodeIterationStatement(
                    new CodeVariableDeclarationStatement(typeof(int), vri.VariableName,
                        new CodeMethodInvokeExpression(fre, "GetLowerBound", ceZero)),
                    new CodeBinaryOperatorExpression(
                        vri, CodeBinaryOperatorType.LessThan,
                        new CodeMethodInvokeExpression(fre, "GetUpperBound", ceZero)),
                    css,

                    new CodeExpressionStatement(
                        new CodeMethodInvokeExpression(ceThis, name, vri, ceFalse))));
            return m;
        }

        static CodeMemberMethod addSetModified(string v, CodeFieldReferenceExpression frMocredVector) {
            //protected override void setModified(int columnIndex, bool isModified) { this._modified[columnIndex] = isModified; }
            CodeMemberMethod m = new CodeMemberMethod();
            CodeArgumentReferenceExpression arIndex, arMod;

            m.Name = v;
            m.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            arIndex = new CodeArgumentReferenceExpression("columnIndex");
            arMod = new CodeArgumentReferenceExpression("isModified");
            m.Parameters.AddRange(new CodeParameterDeclarationExpression[] {
                new CodeParameterDeclarationExpression(typeof(int),arIndex .ParameterName),
                new CodeParameterDeclarationExpression(typeof(bool),arMod .ParameterName)
            });
            m.Statements.Add(new CodeAssignStatement(
                new CodeArrayIndexerExpression(frMocredVector, arIndex), arMod));
            return m;
        }

        static CodeTypeMember addFieldList(CodeTypeReference ctr, string methodName, CodeFieldReferenceExpression cfr) {
            //            protected override List<ColumnDef> fieldList() { return _fields; }
            CodeMemberMethod m = new CodeMemberMethod(); ;

            m.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            m.Name = methodName;
            m.ReturnType = ctr;
            m.Statements.Add(new CodeMethodReturnStatement(cfr));
            return m;
        }
 
        static CodeConstructor addOtherConstructor(CodeGenArgs args, IDataReader reader, CodeTypeDeclaration ctd, CodeDomProvider cdp) {
            CodeConstructor cc;
            CodeArgumentReferenceExpression ar;
            CodeVariableReferenceExpression vr, vrI;
            CodeStatementCollection csc;
            CodeIterationStatement cis;
            string tmp;

            ar = new CodeArgumentReferenceExpression("reader");
            cc = new CodeConstructor();
            cc.Parameters.Add(new CodeParameterDeclarationExpression("IDataReader", ar.ParameterName));
            cc.ChainedConstructorArgs.Add(new CodeSnippetExpression());
            cc.Attributes = MemberAttributes.Public;
            cc.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "ctor"));
            vr = new CodeVariableReferenceExpression("colName");
            vrI = new CodeVariableReferenceExpression("i");
            csc = new CodeStatementCollection();
            addCommentsTo(cc, "field-loading ctor.");

            cc.Comments.Add(addParmComment(ar.ParameterName));

            tmp = removeParensFrom(
                generate(
                    new CodeExpressionStatement(
                        new CodeBinaryOperatorExpression(
                            vrI,
                            CodeBinaryOperatorType.Assign, 
                            new CodeBinaryOperatorExpression(vrI, CodeBinaryOperatorType.Add, ceOne))),
                    cdp, 
                    args.opts));
            CodeStatement csinc;
            csinc = new CodeAssignStatement(
                vrI, new CodeBinaryOperatorExpression(vrI, CodeBinaryOperatorType.Add, ceOne));
            csc.AddRange(
                new CodeStatement[] {
                    new CodeVariableDeclarationStatement(typeof(string),vr.VariableName),
                    csBlank,
                    cis=new CodeIterationStatement(
                        new CodeVariableDeclarationStatement(typeof(int),vrI.VariableName,ceZero),
                        new CodeBinaryOperatorExpression (vrI,  CodeBinaryOperatorType.LessThan ,
                        new CodePropertyReferenceExpression (ar,"FieldCount")),
                        csinc)
                    });

            cis.Statements.AddRange(doSomething(args, reader, ctd, cdp, ar, vrI, vr));
            cc.Statements.AddRange(csc);
            cc.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(ceThis, MNAME_RESET_MOD)));
            return cc;
        }

        static CodeStatementCollection doSomething(CodeGenArgs args, IDataReader reader, CodeTypeDeclaration ctd, CodeDomProvider cdp, CodeArgumentReferenceExpression ar, CodeVariableReferenceExpression vrI, CodeVariableReferenceExpression vr) {
            CodeStatementCollection csc2;
            DataTable dt = reader.GetSchemaTable();
            Type colType;
            CodeMemberField firstField, currField;
            CodeMemberProperty firstProp, currProp;
            bool isCharDatatype;
            string propName, propType, tmp;
            CodeStatement csCN, csswitch;

            csc2 = new CodeStatementCollection();
            csc2 = new CodeStatementCollection();

            csCN = new CodeAssignStatement(vr,
                new CodeMethodInvokeExpression(ar, "GetName", vrI));
            csswitch = new CodeSnippetStatement("\t\t\tswitch(" + removeTrailingChar(generate(csCN, cdp, args.opts), ';') + ") {");
            if (cdp is Microsoft.VisualBasic.VBCodeProvider) {
                csc2.Add(csCN);
                csswitch = new CodeSnippetStatement("\t\t\tselect case " + vr.VariableName);
            }
            csc2.Add(csswitch);

            firstField = currField = null;
            firstProp = currProp = null;
            for (int i = 0; i < reader.FieldCount; i++) {
                isCharDatatype = false;
                tmp = reader.GetName(i);
                propName = makePropName(tmp);
                colType = reader.GetFieldType(i);
                if (colType.Equals(typeof(string)) &&
                    string.Compare(dt.Rows[i]["DataTypeName"].ToString(), "char", true) == 0 &&
                    Convert.ToInt32(dt.Rows[i]["ColumnSize"]) == 1)
                    isCharDatatype = true;
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
                CodeTypeReference ctr0;

                propType = cdp.GetTypeOutput(ctr0=new CodeTypeReference(isCharDatatype ? typeof(char) : colType));
                if (args.generateFields) {
                    currProp = addFieldPropertyPair(ctd, colType, isCharDatatype, tmp, ref currField, i);
                    if (firstProp == null)
                        firstProp = currProp;
                    if (firstField == null)
                        firstField = currField;
                } else {
                    if (cdp is Microsoft.VisualBasic.VBCodeProvider) {
                        CodeMemberProperty p;
                        CodeMemberField f;
                        //CodeTypeReference ctr = new CodeTypeReference(propType);
                        CodeFieldReferenceExpression fr = new CodeFieldReferenceExpression(ceThis, makeFieldName(tmp));

                        //propType = new CodeTypeReference(colType);
                        ctd.Members.AddRange(
                            new CodeTypeMember[] {
                                p = new CodeMemberProperty(),
                                f=new CodeMemberField(ctr0,fr.FieldName)
                            });
                        p.Attributes = MemberAttributes.Public;
                        p.Type = f.Type;
                        p.Name = makePropName(tmp);
                        p.HasSet = true;
                        p.HasGet = true;
                        p.GetStatements.Add(new CodeMethodReturnStatement(fr));
                        p.SetStatements.Add(new CodeAssignStatement(fr, ceValue));

                    } else
                        ctd.Members.Add(new CodeSnippetTypeMember("\t\tpublic " + propType + " " + makePropName(tmp) + " { get; set; }\r\n"));
                    //}
                }
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
                    colType, ar, vrI, cdp, args.opts, isCharDatatype);
            }
            if (currField != null && firstField != null) {
                firstField.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Fields"));
                currField.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Fields"));
            }
            if (currProp != null && firstProp != null) {
                firstProp.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Properties"));
                currProp.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Properties"));
            }

#if FIELD_AND_PROPERTY
            firstProp.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "properties"));
            p.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "properties"));

            firstField.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "fields"));
            f.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "fields"));
#endif
            if (cdp is Microsoft.VisualBasic.VBCodeProvider) {
                csc2.Add(new CodeSnippetStatement("end select"));
            } else
                csc2.Add(new CodeSnippetStatement("\t\t}"));

            //return tmp;
            return csc2;
        }

        static CodeTypeMember createDefaultConstructor() {
            CodeConstructor cc0 = new CodeConstructor();

            cc0.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "ctor"));
            cc0.Attributes = 0;
            addCommentsTo(cc0, "default ctor");
            return cc0;
        }

        static CodeCommentStatement addParmComment(string parmName) {
            return new CodeCommentStatement("<param name=\"" + parmName + "\">");
        }

        static void addCommentsTo(CodeTypeMember ctm, string summaryString) {
            ctm.Comments.Add(createComment("summary", summaryString));
        }

        static CodeCommentStatement createComment(string tag, string summaryString) {
            return new CodeCommentStatement("<" + tag + ">" + summaryString + "</" + tag + ">", true);
        }

        static void implementPropertyChanged(CodeNamespace ns0, CodeTypeDeclaration ctd) {
            const string EVENT_NAME = "PropertyChanged"; ;

            CodeMemberEvent cme;
            CodeEventReferenceExpression cere;
            CodeFieldReferenceExpression cfreNotificationField;
            CodeMemberField fSkipNotify;

            ns0.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            ns0.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            ctd.BaseTypes.Add("INotifyPropertyChanged");
            ctd.Members.Add(cme = implementEvent(EVENT_NAME, out cere));

            ctd.Members.Add(fSkipNotify = createSkipNotificationField());
            cfreNotificationField = new CodeFieldReferenceExpression(null, fSkipNotify.Name);
            implementFireNotification(ctd, cme.Type, cere, cfreNotificationField);
        }

        const string FN_SKIP_NOTIFY = "skipNotification";

        static CodeMemberField createSkipNotificationField() {
            CodeMemberField ret = createStaticField(FN_SKIP_NOTIFY, typeof(bool));

            ret.InitExpression = ceFalse;
            ret.Comments.Add(new CodeCommentStatement("<summary>fix this.</summary>", true));
            return ret;
        }

        #region field-generation methods
        static CodeMemberField createStaticField(string fldName, Type type) {
            return createField(fldName, type, MemberAttributes.Static);

        }

        static CodeMemberField createField(string fldName, Type type, MemberAttributes ma) {
            return createField(fldName, new CodeTypeReference(type), ma);

        }

        static CodeMemberField createField(string fldName, CodeTypeReference ctr, MemberAttributes ma) {
            CodeMemberField ret = new CodeMemberField(ctr, fldName);

            ret.Attributes = ma;
            return ret;
        }

        #endregion

        static void implementFireNotification(CodeTypeDeclaration ctd, CodeTypeReference ctr, CodeEventReferenceExpression cere, CodeFieldReferenceExpression cfre) {
            CodeMemberMethod m;
            CodeArgumentReferenceExpression arPropName;
            CodeStatement csWriteLine;
            CodeConditionStatement csCond;

            const string PARM_NAME_PROP_NAME = "propertyName";

            arPropName = new CodeArgumentReferenceExpression(PARM_NAME_PROP_NAME);
            ctd.Members.Add(m = new CodeMemberMethod());
            m.Attributes = 0;
            m.Name = MNAME_PROP_CHANGED;
            m.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), arPropName.ParameterName));

            csCond = createCondition(new CodeTypeReference("PropertyChangedEventArgs"), cere, arPropName, cfre);
            csWriteLine = createMethodCall(
                        new CodeTypeReferenceExpression("Trace"),
                        "WriteLine",
                        new CodeBinaryOperatorExpression(
                            new CodePrimitiveExpression("property changed:"),
                            CodeBinaryOperatorType.Add,
                            arPropName));
            csCond.TrueStatements.Add(csWriteLine);

            m.Statements.AddRange(
                new CodeStatement[] {
                    csCond,
                });
        }

        static CodeStatement createMethodCall(CodeTypeReferenceExpression ctr, string methodName, params CodeExpression[] exprs) {
            return new CodeExpressionStatement(new CodeMethodInvokeExpression(ctr, methodName, exprs));
        }

        static readonly CodeExpression ceNull = new CodePrimitiveExpression();

        static CodeConditionStatement createCondition(CodeTypeReference ctr, CodeEventReferenceExpression cere, CodeArgumentReferenceExpression arPropName, CodeFieldReferenceExpression cfre) {
            CodeConditionStatement csCond;
            CodeMethodInvokeExpression cmie;
            CodeExpression ceCond;

            cmie = new CodeMethodInvokeExpression(ceThis, cere.EventName, ceThis,
                    new CodeObjectCreateExpression(ctr, arPropName));
            ceCond =
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(
                        cfre,
                        CodeBinaryOperatorType.IdentityEquality,
                        ceFalse),
                    CodeBinaryOperatorType.BooleanAnd,
                    new CodeBinaryOperatorExpression(
                        cere,
                        CodeBinaryOperatorType.IdentityInequality,
                        ceNull));
            csCond = new CodeConditionStatement(ceCond, new CodeExpressionStatement(cmie));
            return csCond;
        }

        static CodeMemberEvent implementEvent(string eventName, out CodeEventReferenceExpression cere) {
            CodeMemberEvent cme;

            cere = new CodeEventReferenceExpression(ceThis, eventName);
            cme = new CodeMemberEvent();
            cme.Attributes = MemberAttributes.Public;
            cme.Name = cere.EventName;
            cme.Type = new CodeTypeReference("PropertyChangedEventHandler ");
            return cme;
        }

        const string MNAME_PROP_CHANGED = "firePropertyChanged";
        static CodeMemberProperty addFieldPropertyPair(CodeTypeDeclaration ctd, Type colType, bool isCharDatatype, string tmp, ref CodeMemberField currField, int fldNo) {
            CodeMemberProperty p;
            CodeMemberField f;
            CodeTypeReference ctr;
            CodeFieldReferenceExpression ce;
            string fldName, propName;

            fldName = makeFieldName(tmp);
            propName = makePropName(tmp);
            ce = new CodeFieldReferenceExpression(ceThis, fldName);
            ctr = new CodeTypeReference(isCharDatatype ? typeof(char) : colType);
            ctd.Members.AddRange(
                new CodeTypeMember[] {
                    currField=f=new CodeMemberField(ctr,ce.FieldName),
                    p=new CodeMemberProperty()
                });
            f.Attributes = 0;
            p.Name = propName;
            p.Type = ctr;
            p.GetStatements.Add(new CodeMethodReturnStatement(ce));
            p.SetStatements.AddRange(
                new CodeStatement[] {
                    new CodeAssignStatement (ce,ceValue ),
                    new CodeExpressionStatement(
                        new CodeMethodInvokeExpression (ceThis,MNAME_PROP_CHANGED,
                        new CodePrimitiveExpression(propName))),
#if false
                    // setting _modified[X]=true
                    new CodeAssignStatement(
                        new CodeArrayIndexerExpression (
                            frModVector,
                            new CodePrimitiveExpression(fldNo)),
                        ceTrue)
#else
                    // ask base-class to set the proper mod-flag
                    new CodeExpressionStatement(
                        new CodeMethodInvokeExpression (ceThis,MNAME_SET_MOD,new CodePrimitiveExpression(tmp)))
#endif
                });
            p.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            return p;
        }

        const string MNAME_SET_MOD = "setModifyFlag";
        const string MNAME_RESET_MOD = "resetModifyFlags";

        static CodeTypeMemberCollection addColtStuff(CodeGenArgs args, CodeTypeDeclaration ctd, CodeNamespace ns,CodeDomProvider cdp) {
            CodeTypeMemberCollection ret = new CodeTypeMemberCollection();
            CodeArgumentReferenceExpression care = new CodeArgumentReferenceExpression("i");

            ret.AddRange(new CodeTypeMember[] {
                implementIsModified(MNAME_IS_MODIFIED),
                implementModifiedProp(PNAME_MODIFIED,ns),
                implementSetupQuery(args, ctd, ns,cdp),
                
                impThisTableName(),
                impNumFields(),

                impFieldAt(care),
                impModAt(care)
            });

            return ret;
        }

          static CodeTypeMember impModAt(CodeArgumentReferenceExpression care) {
            //    protected override bool isModifiedAt(int fieldNo) {
            //    throw new NotImplementedException();
            //}
            CodeMemberMethod ret = new CodeMemberMethod();

            ret.Name = "isModifiedAt";
            ret.ReturnType = new CodeTypeReference(typeof(bool));
            ret.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            ret.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), care.ParameterName));
            ret.Statements.Add(new CodeMethodReturnStatement(
                new CodeArrayIndexerExpression(
                    new CodeFieldReferenceExpression(ceThis, FR_MOD_VECTOR_NAME), care)));
            return ret;
        }

        static CodeTypeMember impFieldAt(CodeArgumentReferenceExpression care) {
            /*
                protected override ColumnDef fieldAt(int fieldNo) {
                    throw new NotImplementedException();
                }
            * */
            CodeMemberMethod ret = new CodeMemberMethod();

            ret.Name = "fieldAt";
            ret.ReturnType = new CodeTypeReference("ColumnDef");
            ret.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            ret.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), care.ParameterName));
            ret.Statements.Add(new CodeMethodReturnStatement(
                new CodeArrayIndexerExpression(
                    //new codeprop
                    new CodeFieldReferenceExpression(null, FNAME_FIELDS), care)));
            return ret;
        }

        static CodeTypeMember impNumFields() {
            CodeMemberMethod ret = new CodeMemberMethod(); ;
            //        public override int numberOfFields() {
            //    throw new NotImplementedException();
            //}
            ret.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            ret.Name = "numberOfFields";
            ret.ReturnType = new CodeTypeReference(typeof(int));
            ret.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodePropertyReferenceExpression(
                        new CodeFieldReferenceExpression(null, FNAME_FIELDS), "Count")));
            return ret;
        }

        static CodeTypeMember impThisTableName() {
            CodeMemberMethod ret = new CodeMemberMethod(); ;

            //  public override string thisTableName() {
            //      throw new NotImplementedException();
            //  }
            ret.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            ret.Name = "thisTableName";
            ret.ReturnType = new CodeTypeReference(typeof(string));
            ret.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(null, "TABLE_NAME")));
            return ret;
        }

        static CodeTypeMember implementSetupQuery(CodeGenArgs args, CodeTypeDeclaration ctd, CodeNamespace ns,CodeDomProvider cdp) {
            CodeMemberMethod ret = new CodeMemberMethod();
            CodeArgumentReferenceExpression arCmd;
            StringBuilder sb;

            arCmd = new CodeArgumentReferenceExpression("cmd");
            ret.Name = "setupQuery";
            ret.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            ret.Parameters.Add(
                new CodeParameterDeclarationExpression(
                    new CodeTypeReference("SqlCommand"),
                    arCmd.ParameterName));

            addImport(ns, "System.Data.SqlClient");
            using (StringWriter sw = new StringWriter(sb = new StringBuilder())) {
                args.provider.GenerateCodeFromMember(ret, sw, args.opts);
            }

            return new CodeSnippetTypeMember(
                makeComment(ctd.Name+"."+ret.Name ,sb.ToString(),cdp));
            //new CodeCommentStatement(
            //    sb.ToString()).Comment.Text);
        }

        static string makeComment(string methodName, string v, CodeDomProvider cdp) {
            string[] parts = v.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //Trace.WriteLine("here");
            StringBuilder sb = new StringBuilder(), sbComment;
            bool isVB = false;

            if (cdp is Microsoft.VisualBasic.VBCodeProvider)
                isVB = true;

            if (!isVB)
                sb.AppendLine("#warning implement or remove " + methodName + " as needed.");
            else
                sb.AppendLine();
            sbComment = new StringBuilder();
            foreach (string apart in parts)
                sbComment.AppendLine(apart);
            //sb.AppendLine("\t\t// " + apart);
            var avar = new CodeCommentStatement(sbComment.ToString());
            using (StringWriter sw = new StringWriter(sb)) {
                cdp.GenerateCodeFromStatement(avar, sw, null);
            }
            return sb.ToString();
        }

        static void addImport(CodeNamespace ns, string anImport) {
            if (ns == null)
                throw new ArgumentNullException("CodeNamespace is null!", "ns");
            if (string.IsNullOrEmpty(anImport ))
                throw new ArgumentNullException("import is null!", "anImport");
            foreach(CodeNamespaceImport cni in ns.Imports) 
                if (string.Compare(cni.Namespace, anImport) == 0)
                    return;
            ns.Imports.Add(new CodeNamespaceImport(anImport));
        }

        const string MNAME_IS_MODIFIED = "isModified";
        const string PNAME_MODIFIED = "modified";
        static CodeTypeMember implementModifiedProp(string v, CodeNamespace ns) {
            CodeMemberProperty p = new CodeMemberProperty();

            /*
             * [Browsable(false)]
                public override bool modified { get { return isModified(); } }
             * */
            p.Name = v;
            p.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            p.Type = new CodeTypeReference(typeof(bool));
            p.HasGet = true;
            p.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(null, MNAME_IS_MODIFIED)));
            p.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference("Browsable"), new CodeAttributeArgument(ceFalse)));
            addImport(ns, "System.ComponentModel");
            return p;
        }

        static CodeTypeMember implementIsModified(string methodName) {
            CodeMemberMethod m = new CodeMemberMethod();
            //CodeIterationStatement cis;
            CodeVariableReferenceExpression vri = new CodeVariableReferenceExpression("i");
            CodeFieldReferenceExpression frMod = new CodeFieldReferenceExpression(null, FR_MOD_VECTOR_NAME);

            m.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            m.Name = methodName;
            m.ReturnType = new CodeTypeReference(typeof(bool));

            m.Statements.AddRange(
                new CodeStatement[] {
                    new CodeIterationStatement(
                        new CodeVariableDeclarationStatement(typeof(int), vri.VariableName,
                        new CodeMethodInvokeExpression(frMod, "GetLowerBound", ceZero)),

                        new CodeBinaryOperatorExpression(vri, CodeBinaryOperatorType.LessThan,
                        new CodeMethodInvokeExpression(frMod, "GetUpperBound", ceZero)),
                        new CodeAssignStatement(vri,
                            new CodeBinaryOperatorExpression(vri, CodeBinaryOperatorType.Add, ceOne)),
                        new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodeArrayIndexerExpression(frMod, vri),
                                  CodeBinaryOperatorType.ValueEquality,
                                  ceTrue),
                            new CodeMethodReturnStatement(ceTrue))),
                    new CodeMethodReturnStatement(ceFalse)
                });
            /*
             *  for (int i = _modified.GetLowerBound(0); i < _modified.GetUpperBound(0); i++)
                if (_modified[i])
                    return true;
            return false;
             * */
            return m;
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

        static void generateCSVFrom(IDataReader reader, string tableName, string outDir, CodeTypeDeclaration ctd, CodeNamespace ns,CodeDomProvider cdp) {
            DataTable dt = reader.GetSchemaTable();

            generateColumnCollection(ctd, ns, dt,cdp);
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

        const string FNAME_FIELDS = "_fields";
        static void generateColumnCollection(CodeTypeDeclaration ctd, CodeNamespace ns, DataTable dt,CodeDomProvider cdp) {
            CodeTypeReference ctrColDef = new CodeTypeReference("ColumnDef");
            CodeExpression[] args = makeClassFieldCollection(dt, ctrColDef);
            CodeMemberField f, f3;
            CodeMemberProperty f2;

            ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            ctd.Members.Add(f = new CodeMemberField());
            f.Attributes = MemberAttributes.Static;
            f.Name = FNAME_FIELDS;
            f.Type = new CodeTypeReference("List", ctrColDef);
            f.InitExpression = new CodeObjectCreateExpression(f.Type, new CodeArrayCreateExpression(ctrColDef, args));

            f3 = new CodeMemberField(new CodeTypeReference(new CodeTypeReference(typeof(bool)), 1), FR_MOD_VECTOR_NAME);
            f3.Attributes = 0;
            if (cdp is Microsoft.VisualBasic.VBCodeProvider)
                f3.Attributes = MemberAttributes.Private;
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
            CodeArgumentReferenceExpression ar, CodeVariableReferenceExpression vrI, CodeDomProvider cdp, CodeGeneratorOptions opts, bool isCharDatatype) {
            CodeStatement cs;
            CodeMethodInvokeExpression cmie;
            CodeExpression ceNullForType;

            cmie = new CodeMethodInvokeExpression(ar, findTypeMethod(colType, cdp), vrI);
            if (isCharDatatype) {
                cs = new CodeAssignStatement(propRef, new CodeIndexerExpression(cmie, ceZero));
                ceNullForType = makeNullAssignment(typeof(char), cdp);
            } else {
                cs = new CodeAssignStatement(propRef, cmie);
                ceNullForType = makeNullAssignment(colType, cdp);
            }
            csc.AddRange(new CodeStatement[] {
                new CodeSnippetStatement("\t\tcase \"" + fieldName + "\":"),
                new CodeAssignStatement(propRef, ceNullForType),
                new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeMethodInvokeExpression(ar, "IsDBNull", vrI), CodeBinaryOperatorType.IdentityInequality, ceTrue),
                        new CodeCommentStatement("do real assign here"),
                        cs)
            });
            if (!(cdp is Microsoft.VisualBasic.VBCodeProvider))
                csc.Add(csBreak);
        }

        static CodeExpression makeNullAssignment(Type colType, CodeDomProvider cdp) {
            string outType = cdp.GetTypeOutput(new CodeTypeReference(colType));

            if (string.Compare(outType, "string", true) == 0) return new CodeSnippetExpression(outType + ".Empty");
            else if (string.Compare(outType, "int", true) == 0) return new CodeSnippetExpression("int.MinValue");
            else if (string.Compare(outType, "integer", true) == 0) return new CodeSnippetExpression("Integer.MinValue");
            else if (string.Compare(outType, "char", true) == 0) return new CodeSnippetExpression("char.MinValue");
            else if (string.Compare(outType, "long", true) == 0) return new CodeSnippetExpression("long.MinValue");
            else if (string.Compare(outType, "bool", true) == 0) return new CodeSnippetExpression("false");
            else if (string.Compare(outType, "boolean", true) == 0) return new CodeSnippetExpression("false");
            else if (string.Compare(outType, "decimal", true) == 0) return new CodeSnippetExpression("Decimal.MinValue");
            else if (string.Compare(outType, "double", true) == 0) return new CodeSnippetExpression("Double.MinValue");
            else if (string.Compare(outType, "System.Guid", true) == 0) return new CodeSnippetExpression("Guid.Empty");
            else if (string.Compare(outType, "System.DateTime", true) == 0) return new CodeSnippetExpression("DateTime.MinValue");
            else if (string.Compare(outType, "Date", true) == 0) return new CodeSnippetExpression("DateTime.MinValue");
            throw new InvalidOperationException("Unhandled type: '" + outType + "' [" + colType.FullName + "]");
        }

        static string findTypeMethod(Type colType, CodeDomProvider cdp) {
            string outType = cdp.GetTypeOutput(new CodeTypeReference(colType));

            if (string.Compare(outType, "string", true) == 0) return "GetString";
            else if (string.Compare(outType, "char", true) == 0) return "GetChar";
            else if (string.Compare(outType, "int", true) == 0) return "GetInt32";
            else if (string.Compare(outType, "Integer", true) == 0) return "GetInt32";
            else if (string.Compare(outType, "long", true) == 0) return "GetInt64";
            else if (string.Compare(outType, "bool", true) == 0) return "GetBoolean";
            else if (string.Compare(outType, "boolean", true) == 0) return "GetBoolean";
            else if (string.Compare(outType, "decimal", true) == 0) return "GetDecimal";
            else if (string.Compare(outType, "double", true) == 0) return "GetDouble";
            else if (string.Compare(outType, "System.Guid", true) == 0) return "GetGuid";
            else if (string.Compare(outType, "System.DateTime", true) == 0) return "GetDateTime";
            else if (string.Compare(outType, "Date", true) == 0) return "GetDateTime";
            throw new InvalidOperationException("Unhandled type: '" +outType+"' ["+ colType.FullName+"]");
        }

        static string generate(CodeStatement cs, CodeDomProvider cdp, CodeGeneratorOptions opts) {
            StringBuilder sb = new StringBuilder();

            using (StringWriter sw = new StringWriter(sb)) {
                cdp.GenerateCodeFromStatement(cs, sw, opts);
            }
            return sb.ToString();
        }

        static CodeMemberProperty makeProperty(string propName, CodeMemberField f, int modVectorIndex) {
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
            return fixup(procName, true);
        }
    }
}