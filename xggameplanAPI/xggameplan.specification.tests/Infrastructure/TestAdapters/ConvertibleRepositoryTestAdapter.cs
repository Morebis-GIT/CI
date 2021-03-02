using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoFixture;
using AutoFixture.Dsl;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using xggameplan.specification.tests.Exceptions;
using xggameplan.specification.tests.Extensions;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.TestAdapters
{
    public abstract class ConvertibleRepositoryTestAdapter<TRepositoryModel, TTestModel, TRepository, TIdType> :
        ConvertibleRepositoryTestAdapter<TRepositoryModel, TTestModel, TRepository, TIdType, RepositoryTestContext>
        where TRepositoryModel : class
        where TTestModel : class
        where TRepository : class
    {
        protected ConvertibleRepositoryTestAdapter(IScenarioDbContext dbContext, TRepository repository) : base(dbContext,
            repository)
        {
        }
    }

    public abstract class ConvertibleRepositoryTestAdapter<TRepositoryModel, TTestModel, TRepository, TIdType, TTestContext> : IRepositoryAdapter
        where TRepositoryModel : class
        where TTestModel : class
        where TRepository : class
        where TTestContext : RepositoryTestContext, new()
    {
        private IRepositoryMethodExecutor _repositoryMethodExecutor;

        protected Fixture Fixture { get; } = new SafeFixture();

        protected TRepository Repository { get; }

        protected IScenarioDbContext DbContext { get; }

        protected ConvertibleRepositoryTestAdapter(IScenarioDbContext dbContext, TRepository repository)
        {
            DbContext = dbContext;
            Repository = repository;
            TestContext = new TTestContext
            {
                RepositoryModelType = typeof(TRepositoryModel),
                TestModelType = typeof(TRepositoryModel)
            };
        }

        protected virtual IRepositoryMethodExecutor RepositoryMethodExecutor =>
            _repositoryMethodExecutor ?? (_repositoryMethodExecutor =
                new DefaultRepositoryMethodExecutor<TRepository>(Repository, this));

        protected abstract ITestModelConverter<TRepositoryModel, TTestModel> ModelConverter { get; }

        protected abstract TRepositoryModel Add(TRepositoryModel model);

        protected abstract IEnumerable<TRepositoryModel> AddRange(params TRepositoryModel[] models);

        protected abstract TRepositoryModel Update(TRepositoryModel model);

        protected abstract TRepositoryModel GetById(TIdType id);

        protected abstract IEnumerable<TRepositoryModel> GetAll();

        protected abstract void Delete(TIdType id);

        protected abstract void Truncate();

        protected abstract int Count();

        protected virtual IPostprocessComposer<TTestModel> GetAutoModelComposer()
        {
            return Fixture.Build<TTestModel>().WithAutoProperties();
        }

        protected virtual TIdType ConvertIdValue(string id)
        {
            return id.SpecflowConvert<TIdType>();
        }

        protected void AssignTestContextSingleResult(TTestModel model)
        {
            TestContext.LastOperationCount = model is null ? 0 : 1;
            TestContext.LastSingleResult = model;
            TestContext.LastCollectionResult = model is null
                ? null
                : new List<object> { model };
        }

        protected void AssignTestContextCollectionResult(IEnumerable<TTestModel> collection)
        {
            TestContext.LastCollectionResult = collection?.Cast<object>().ToList();
            TestContext.LastOperationCount = collection?.Count() ?? 0;
            TestContext.LastSingleResult = TestContext.LastOperationCount == 1
                ? collection.First()
                : null;
        }

        protected void AssignTestContextTupleResult(ITuple tuple)
        {
            TestContext.LastCollectionResult = null;
            TestContext.LastSingleResult = null;

            if (tuple is null)
            {
                TestContext.LastOperationCount = 0;

                return;
            }

            int operationCount = 0;

            for (int i = 0; i < tuple.Length; i++)
            {
                switch (tuple[i])
                {
                    case IEnumerable item:
                        operationCount += Count(item);
                        break;

                    case null:
                        break;

                    default:
                        operationCount++;
                        break;
                }
            }

            TestContext.LastOperationCount = operationCount;
        }

        private static int Count(IEnumerable item)
        {
            int operationCount = 0;
            var enumerator = item.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    operationCount++;
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            return operationCount;
        }

        protected RepositoryExecuteResult ExecuteAdapterAction(Action action)
        {
            TestContext.LastException = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                TestContext.LastException = ex;
                TestContext.LastOperationCount = null;
                TestContext.LastCollectionResult = null;
                TestContext.LastSingleResult = null;
            }

            return new RepositoryExecuteResult(TestContext);
        }

        public TTestContext TestContext { get; protected set; }

        public RepositoryExecuteResult ExecuteAutoAdd(int count)
        {
            return ExecuteAdapterAction(() =>
            {
                var res = new List<TRepositoryModel>();

                if (count > 0)
                {
                    DbContext.WaitForIndexesAfterSaveChanges();
                    if (count == 1)
                    {
                        res.Add(Add(ModelConverter.ConvertToRepositoryModel(GetAutoModelComposer().Create())));
                    }
                    else
                    {
                        res.AddRange(AddRange(GetAutoModelComposer().CreateMany(count)
                            .Select(ModelConverter.ConvertToRepositoryModel).ToArray()).ToList());
                    }

                    DbContext.SaveChanges();
                }

                TestContext.LastOperationCount = res.Count;
                TestContext.LastCollectionResult = count > 1 ? res.Cast<object>().ToList() : null;
                TestContext.LastSingleResult = res.Count == 1 ? res[0] : null;
            });
        }

        public RepositoryExecuteResult ExecuteGivenAutoAdd(int count)
        {
            return ExecuteAdapterAction(() =>
            {
                if (count == 0)
                {
                    return;
                }

                DbContext.WaitForIndexesAfterSaveChanges();

                if (count == 1)
                {
                    _ = DbContext.Add(
                            ModelConverter.ConvertToRepositoryModel(
                                GetAutoModelComposer().Create()
                            ));
                }
                else
                {
                    DbContext.AddRange(
                        GetAutoModelComposer()
                            .CreateMany(count)
                            .Select(ModelConverter.ConvertToRepositoryModel)
                            .ToArray()
                        );
                }

                DbContext.SaveChanges();
            });
        }

        public RepositoryExecuteResult ExecuteGivenAdd(Table values)
        {
            return ExecuteAdapterAction(() =>
            {
                var model = ModelConverter.ConvertToRepositoryModel(values.CreateInstance<TTestModel>());
                DbContext.WaitForIndexesAfterSaveChanges();
                _ = DbContext.Add(model);
                DbContext.SaveChanges();
            });
        }

        public RepositoryExecuteResult ExecuteGivenAddRange(Table entities)
        {
            return ExecuteAdapterAction(() =>
            {
                var models = entities
                    .CreateSet<TTestModel>()
                    .Select(ModelConverter.ConvertToRepositoryModel)
                    .ToArray();

                DbContext.WaitForIndexesAfterSaveChanges();
                DbContext.AddRange(models);
                DbContext.SaveChanges();
            });
        }

        public RepositoryExecuteResult ExecuteAdd(Table values)
        {
            return ExecuteAdapterAction(() =>
            {
                var model = values.CreateInstance<TTestModel>();
                DbContext.WaitForIndexesAfterSaveChanges();
                _ = Add(ModelConverter.ConvertToRepositoryModel(model));
                DbContext.SaveChanges();

                AssignTestContextSingleResult(model);
            });
        }

        public RepositoryExecuteResult ExecuteAddRange(Table entities)
        {
            return ExecuteAdapterAction(() =>
            {
                var models = entities.CreateSet<TTestModel>().ToArray();
                DbContext.WaitForIndexesAfterSaveChanges();
                _ = AddRange(models.Select(ModelConverter.ConvertToRepositoryModel).ToArray());
                DbContext.SaveChanges();

                AssignTestContextCollectionResult(models);
            });
        }

        public RepositoryExecuteResult ExecuteUpdate(Table values)
        {
            return ExecuteAdapterAction(() =>
            {
                if (TestContext.LastSingleResult is null)
                {
                    throw new RepositoryAdapterException("There is no model to update.");
                }

                if (!(TestContext.LastSingleResult is TTestModel model))
                {
                    throw new RepositoryAdapterException("Updating model has different type.");
                }

                values.FillInstance(model);
                DbContext.WaitForIndexesAfterSaveChanges();
                _ = Update(ModelConverter.ConvertToRepositoryModel(model));
                DbContext.SaveChanges();

                TestContext.LastSingleResult = model;
            });
        }

        public RepositoryExecuteResult ExecuteGetAll()
        {
            return ExecuteAdapterAction(() =>
            {
                var res = GetAll().Select(ModelConverter.ConvertToTestModel).ToList();
                AssignTestContextCollectionResult(res);
            });
        }

        public RepositoryExecuteResult ExecuteGetById(string id)
        {
            return ExecuteAdapterAction(() =>
            {
                var res = ModelConverter.ConvertToTestModel(GetById(ConvertIdValue(id)));
                AssignTestContextSingleResult(res);
            });
        }

        public RepositoryExecuteResult ExecuteDelete(string id)
        {
            return ExecuteAdapterAction(() =>
            {
                DbContext.WaitForIndexesAfterSaveChanges();
                Delete(ConvertIdValue(id));
                DbContext.SaveChanges();

                TestContext.LastOperationCount = null;
                TestContext.LastSingleResult = null;
                TestContext.LastCollectionResult = null;
            });
        }

        public RepositoryExecuteResult ExecuteTruncate()
        {
            return ExecuteAdapterAction(() =>
            {
                DbContext.WaitForIndexesAfterSaveChanges();
                Truncate();
                DbContext.SaveChanges();

                TestContext.LastOperationCount = null;
                TestContext.LastSingleResult = null;
                TestContext.LastCollectionResult = null;
            });
        }

        public RepositoryExecuteResult ExecuteCount()
        {
            return ExecuteAdapterAction(() =>
            {
                TestContext.LastOperationCount = Count();
                TestContext.LastSingleResult = null;
                TestContext.LastCollectionResult = null;
            });
        }

        public RepositoryExecuteResult ExecuteMethod(string methodName, Table parameters = null)
        {
            return ExecuteAdapterAction(() =>
            {
                var callResult =
                    RepositoryMethodExecutor.Execute(methodName, parameters?.ToRepositoryMethodParameters());

                if (callResult.ResultIsHandled)
                {
                    return;
                }

                if (typeof(TTestModel).IsAssignableFrom(callResult.ResultType))
                {
                    AssignTestContextSingleResult(callResult.Result as TTestModel);
                    return;
                }

                if (typeof(TRepositoryModel).IsAssignableFrom(callResult.ResultType))
                {
                    AssignTestContextSingleResult(
                        ModelConverter.ConvertToTestModel(callResult.Result as TRepositoryModel));
                    return;
                }

                if (typeof(IEnumerable<TTestModel>).IsAssignableFrom(callResult.ResultType))
                {
                    AssignTestContextCollectionResult(callResult.Result as IEnumerable<TTestModel>);
                    return;
                }

                if (typeof(IEnumerable<TRepositoryModel>).IsAssignableFrom(callResult.ResultType))
                {
                    AssignTestContextCollectionResult(
                        ((IEnumerable<TRepositoryModel>)callResult.Result).Select(ModelConverter.ConvertToTestModel));
                    return;
                }

                if (typeof(ITuple).IsAssignableFrom(callResult.ResultType))
                {
                    AssignTestContextTupleResult(callResult.Result as ITuple);
                    return;
                }

                Type enumerableInterfaceType = null;
                if (callResult.ResultType.IsInterface && callResult.ResultType.IsGenericType &&
                    typeof(IEnumerable<>).IsAssignableFrom(callResult.ResultType.GetGenericTypeDefinition()))
                {
                    enumerableInterfaceType = callResult.ResultType;
                }

                if (enumerableInterfaceType is null)
                {
                    enumerableInterfaceType = callResult.ResultType.GetInterface(typeof(IEnumerable<>).Name);
                }

                if (enumerableInterfaceType != null)
                {
                    var genericType = enumerableInterfaceType.GenericTypeArguments[0];
                    if (typeof(TTestModel).IsAssignableFrom(genericType))
                    {
                        AssignTestContextCollectionResult(
                            ((IEnumerable)callResult.Result).Cast<TTestModel>()
                            );

                        return;
                    }

                    if (typeof(TRepositoryModel).IsAssignableFrom(genericType))
                    {
                        AssignTestContextCollectionResult(
                            ((IEnumerable)callResult.Result)
                            .Cast<TRepositoryModel>()
                            .Select(ModelConverter.ConvertToTestModel)
                            );

                        return;
                    }

                    var result = (callResult.Result as IEnumerable)?.Cast<object>().ToList();
                    TestContext.LastOperationCount = result?.Count ?? 0;
                    TestContext.LastCollectionResult = result;
                    TestContext.LastSingleResult = TestContext.LastOperationCount == 1
                        ? result[0]
                        : null;

                    return;
                }

                TestContext.LastOperationCount = null;
                TestContext.LastSingleResult = callResult.Result;
                TestContext.LastCollectionResult = null;
            });
        }

        public void CheckReceivedResult(Table values)
        {
            if (TestContext.LastSingleResult is null)
            {
                throw new RepositoryAdapterException("There is no model to check.");
            }

            values.CompareToInstance(
                TestContext.LastSingleResult,
                TestContext.LastSingleResult.GetType()
                );
        }

        public RepositoryTestContext GetTestContext() =>
            GetTestContext<RepositoryTestContext>();

        public TTestContextType GetTestContext<TTestContextType>()
            where TTestContextType : RepositoryTestContext
        {
            if (typeof(TTestContextType).IsAssignableFrom(typeof(TTestContext)))
            {
                return TestContext as TTestContextType;
            }

            throw new Exception(
                typeof(TTestContext).Name + " test context type can't be " +
                $"returned as requested {typeof(TTestContextType).Name} type."
                );
        }
    }
}
