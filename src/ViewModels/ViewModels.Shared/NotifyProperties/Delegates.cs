using System;
using System.Threading.Tasks;

namespace Das.ViewModels;

public delegate Boolean AllowPropertyChangeDelegate<in T>(T? oldValue,
                                                          T? newValue);

public delegate T? InterceptPropertyChangeDelegate<T>(T? oldValue,
                                                      T? newValue);
