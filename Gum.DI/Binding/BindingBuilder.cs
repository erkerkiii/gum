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

		public BindingStrategyBuilder FromInstance<TObject>(TObject instance) where TObject : TBinding
		{
			_pendingBindingInfo.objectType = typeof(TObject);
			_pendingBindingInfo.instance = instance;
			return new BindingStrategyBuilder(_pendingBindingInfo);
		}
	}

	public ref struct BindingStrategyBuilder
	{
		private PendingBindingInfo _pendingBindingInfo;
		
		internal BindingStrategyBuilder(PendingBindingInfo pendingBindingInfo)
		{
			_pendingBindingInfo = pendingBindingInfo;
		}

		public void AsSingle()
		{
			_pendingBindingInfo.bindingStrategy = BindingStrategy.Single;
			_pendingBindingInfo.diContainer.CompleteBinding(_pendingBindingInfo);
		}
		
		public void AsTransient()
		{
			_pendingBindingInfo.bindingStrategy = BindingStrategy.Transient;
			_pendingBindingInfo.diContainer.CompleteBinding(_pendingBindingInfo);
		}
		
		public void AsCached()
		{
			_pendingBindingInfo.bindingStrategy = BindingStrategy.Cached;
			_pendingBindingInfo.diContainer.CompleteBinding(_pendingBindingInfo);
		}
	}
}