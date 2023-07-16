namespace Gum.DI.Binding
{
	public ref struct ObjectTypeBuilder<TBinding>
	{
		private PendingBindingInfo _pendingBindingInfo;
		
		internal ObjectTypeBuilder(PendingBindingInfo pendingBindingInfo)
		{
			_pendingBindingInfo = pendingBindingInfo;
		}

		public BindingStrategyBuilder To<TObject>() where TObject : TBinding
		{
			_pendingBindingInfo.objectType = typeof(TObject);
			return new BindingStrategyBuilder(_pendingBindingInfo);
		}

		public void FromInstance<TObject>(TObject instance) where TObject : TBinding
		{
			_pendingBindingInfo.objectType = typeof(TObject);
			_pendingBindingInfo.instance = instance;
			_pendingBindingInfo.bindingStrategy = BindingStrategy.Single;
			_pendingBindingInfo.CompleteBinding();
		}
	}

	public ref struct BindingStrategyBuilder
	{
		private PendingBindingInfo _pendingBindingInfo;
		
		internal BindingStrategyBuilder(PendingBindingInfo pendingBindingInfo)
		{
			_pendingBindingInfo = pendingBindingInfo;
		}

		public InstantiationStrategyBuilder AsSingle()
		{
			_pendingBindingInfo.bindingStrategy = BindingStrategy.Single;
			return new InstantiationStrategyBuilder(_pendingBindingInfo);
		}
		
		public InstantiationStrategyBuilder AsTransient()
		{
			_pendingBindingInfo.bindingStrategy = BindingStrategy.Transient;
			return new InstantiationStrategyBuilder(_pendingBindingInfo);
		}
		
		public InstantiationStrategyBuilder AsCached()
		{
			_pendingBindingInfo.bindingStrategy = BindingStrategy.Cached;
			return new InstantiationStrategyBuilder(_pendingBindingInfo);
		}
	}
	
	public ref struct InstantiationStrategyBuilder
	{
		private PendingBindingInfo _pendingBindingInfo;
		
		internal InstantiationStrategyBuilder(PendingBindingInfo pendingBindingInfo)
		{
			_pendingBindingInfo = pendingBindingInfo;
		}

		public void Lazy()
		{
			_pendingBindingInfo.isLazy = true;
			_pendingBindingInfo.CompleteBinding();
		}
		
		public void Eager()
		{
			_pendingBindingInfo.isLazy = false;
			_pendingBindingInfo.CompleteBinding();
		}
	}
}