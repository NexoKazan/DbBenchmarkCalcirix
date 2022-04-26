package org.apache.calcite.adapter.file;

import com.google.common.base.MoreObjects;
import com.google.errorprone.annotations.CanIgnoreReturnValue;
import com.google.errorprone.annotations.Var;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import javax.annotation.CheckReturnValue;
import javax.annotation.Nullable;
import javax.annotation.ParametersAreNonnullByDefault;
import javax.annotation.concurrent.Immutable;
import javax.annotation.concurrent.NotThreadSafe;
import org.apache.calcite.plan.RelRule;
import org.apache.calcite.tools.RelBuilderFactory;
import org.immutables.value.Generated;

/**
 * {@code ImmutableCsvProjectTableScanRule} contains immutable implementation classes generated from
 * abstract value types defined as nested inside {@link CsvProjectTableScanRule}.
 * @see ImmutableCsvProjectTableScanRule.Config
 */
@Generated(from = "CsvProjectTableScanRule", generator = "Immutables")
@SuppressWarnings({"all"})
@ParametersAreNonnullByDefault
@javax.annotation.processing.Generated("org.immutables.processor.ProxyProcessor")
final class ImmutableCsvProjectTableScanRule {
  private ImmutableCsvProjectTableScanRule() {}

  /**
   * Immutable implementation of {@link CsvProjectTableScanRule.Config}.
   * <p>
   * Use the builder to create immutable instances:
   * {@code ImmutableCsvProjectTableScanRule.Config.builder()}.
   */
  @Generated(from = "CsvProjectTableScanRule.Config", generator = "Immutables")
  @Immutable
  @CheckReturnValue
  static final class Config implements CsvProjectTableScanRule.Config {
    private final RelBuilderFactory relBuilderFactory;
    private final @Nullable String description;
    private final RelRule.OperandTransform operandSupplier;

    private Config(ImmutableCsvProjectTableScanRule.Config.Builder builder) {
      this.description = builder.description;
      if (builder.relBuilderFactory != null) {
        initShim.withRelBuilderFactory(builder.relBuilderFactory);
      }
      if (builder.operandSupplier != null) {
        initShim.withOperandSupplier(builder.operandSupplier);
      }
      this.relBuilderFactory = initShim.relBuilderFactory();
      this.operandSupplier = initShim.operandSupplier();
      this.initShim = null;
    }

    private Config(
        RelBuilderFactory relBuilderFactory,
        @Nullable String description,
        RelRule.OperandTransform operandSupplier) {
      this.relBuilderFactory = relBuilderFactory;
      this.description = description;
      this.operandSupplier = operandSupplier;
      this.initShim = null;
    }

    private static final byte STAGE_INITIALIZING = -1;
    private static final byte STAGE_UNINITIALIZED = 0;
    private static final byte STAGE_INITIALIZED = 1;
    @SuppressWarnings("Immutable")
    private transient volatile InitShim initShim = new InitShim();

    @Generated(from = "CsvProjectTableScanRule.Config", generator = "Immutables")
    private final class InitShim {
      private byte relBuilderFactoryBuildStage = STAGE_UNINITIALIZED;
      private RelBuilderFactory relBuilderFactory;

      RelBuilderFactory relBuilderFactory() {
        if (relBuilderFactoryBuildStage == STAGE_INITIALIZING) throw new IllegalStateException(formatInitCycleMessage());
        if (relBuilderFactoryBuildStage == STAGE_UNINITIALIZED) {
          relBuilderFactoryBuildStage = STAGE_INITIALIZING;
          this.relBuilderFactory = Objects.requireNonNull(relBuilderFactoryInitialize(), "relBuilderFactory");
          relBuilderFactoryBuildStage = STAGE_INITIALIZED;
        }
        return this.relBuilderFactory;
      }

      void withRelBuilderFactory(RelBuilderFactory relBuilderFactory) {
        this.relBuilderFactory = relBuilderFactory;
        relBuilderFactoryBuildStage = STAGE_INITIALIZED;
      }

      private byte operandSupplierBuildStage = STAGE_UNINITIALIZED;
      private RelRule.OperandTransform operandSupplier;

      RelRule.OperandTransform operandSupplier() {
        if (operandSupplierBuildStage == STAGE_INITIALIZING) throw new IllegalStateException(formatInitCycleMessage());
        if (operandSupplierBuildStage == STAGE_UNINITIALIZED) {
          operandSupplierBuildStage = STAGE_INITIALIZING;
          this.operandSupplier = Objects.requireNonNull(operandSupplierInitialize(), "operandSupplier");
          operandSupplierBuildStage = STAGE_INITIALIZED;
        }
        return this.operandSupplier;
      }

      void withOperandSupplier(RelRule.OperandTransform operandSupplier) {
        this.operandSupplier = operandSupplier;
        operandSupplierBuildStage = STAGE_INITIALIZED;
      }

      private String formatInitCycleMessage() {
        List<String> attributes = new ArrayList<>();
        if (relBuilderFactoryBuildStage == STAGE_INITIALIZING) attributes.add("relBuilderFactory");
        if (operandSupplierBuildStage == STAGE_INITIALIZING) attributes.add("operandSupplier");
        return "Cannot build Config, attribute initializers form cycle " + attributes;
      }
    }

    private RelBuilderFactory relBuilderFactoryInitialize() {
      return CsvProjectTableScanRule.Config.super.relBuilderFactory();
    }

    private RelRule.OperandTransform operandSupplierInitialize() {
      return CsvProjectTableScanRule.Config.super.operandSupplier();
    }

    /**
     * @return The value of the {@code relBuilderFactory} attribute
     */
    @Override
    public RelBuilderFactory relBuilderFactory() {
      InitShim shim = this.initShim;
      return shim != null
          ? shim.relBuilderFactory()
          : this.relBuilderFactory;
    }

    /**
     * @return The value of the {@code description} attribute
     */
    @Override
    public @Nullable String description() {
      return description;
    }

    /**
     * @return The value of the {@code operandSupplier} attribute
     */
    @Override
    public RelRule.OperandTransform operandSupplier() {
      InitShim shim = this.initShim;
      return shim != null
          ? shim.operandSupplier()
          : this.operandSupplier;
    }

    /**
     * Copy the current immutable object by setting a value for the {@link CsvProjectTableScanRule.Config#relBuilderFactory() relBuilderFactory} attribute.
     * A shallow reference equality check is used to prevent copying of the same value by returning {@code this}.
     * @param value A new value for relBuilderFactory
     * @return A modified copy of the {@code this} object
     */
    public final ImmutableCsvProjectTableScanRule.Config withRelBuilderFactory(RelBuilderFactory value) {
      if (this.relBuilderFactory == value) return this;
      RelBuilderFactory newValue = Objects.requireNonNull(value, "relBuilderFactory");
      return new ImmutableCsvProjectTableScanRule.Config(newValue, this.description, this.operandSupplier);
    }

    /**
     * Copy the current immutable object by setting a value for the {@link CsvProjectTableScanRule.Config#description() description} attribute.
     * An equals check used to prevent copying of the same value by returning {@code this}.
     * @param value A new value for description (can be {@code null})
     * @return A modified copy of the {@code this} object
     */
    public final ImmutableCsvProjectTableScanRule.Config withDescription(@Nullable String value) {
      if (Objects.equals(this.description, value)) return this;
      return new ImmutableCsvProjectTableScanRule.Config(this.relBuilderFactory, value, this.operandSupplier);
    }

    /**
     * Copy the current immutable object by setting a value for the {@link CsvProjectTableScanRule.Config#operandSupplier() operandSupplier} attribute.
     * A shallow reference equality check is used to prevent copying of the same value by returning {@code this}.
     * @param value A new value for operandSupplier
     * @return A modified copy of the {@code this} object
     */
    public final ImmutableCsvProjectTableScanRule.Config withOperandSupplier(RelRule.OperandTransform value) {
      if (this.operandSupplier == value) return this;
      RelRule.OperandTransform newValue = Objects.requireNonNull(value, "operandSupplier");
      return new ImmutableCsvProjectTableScanRule.Config(this.relBuilderFactory, this.description, newValue);
    }

    /**
     * This instance is equal to all instances of {@code Config} that have equal attribute values.
     * @return {@code true} if {@code this} is equal to {@code another} instance
     */
    @Override
    public boolean equals(@Nullable Object another) {
      if (this == another) return true;
      return another instanceof ImmutableCsvProjectTableScanRule.Config
          && equalTo((ImmutableCsvProjectTableScanRule.Config) another);
    }

    private boolean equalTo(ImmutableCsvProjectTableScanRule.Config another) {
      return relBuilderFactory.equals(another.relBuilderFactory)
          && Objects.equals(description, another.description)
          && operandSupplier.equals(another.operandSupplier);
    }

    /**
     * Computes a hash code from attributes: {@code relBuilderFactory}, {@code description}, {@code operandSupplier}.
     * @return hashCode value
     */
    @Override
    public int hashCode() {
      @Var int h = 5381;
      h += (h << 5) + relBuilderFactory.hashCode();
      h += (h << 5) + Objects.hashCode(description);
      h += (h << 5) + operandSupplier.hashCode();
      return h;
    }

    /**
     * Prints the immutable value {@code Config} with attribute values.
     * @return A string representation of the value
     */
    @Override
    public String toString() {
      return MoreObjects.toStringHelper("Config")
          .omitNullValues()
          .add("relBuilderFactory", relBuilderFactory)
          .add("description", description)
          .add("operandSupplier", operandSupplier)
          .toString();
    }

    /**
     * Creates an immutable copy of a {@link CsvProjectTableScanRule.Config} value.
     * Uses accessors to get values to initialize the new immutable instance.
     * If an instance is already immutable, it is returned as is.
     * @param instance The instance to copy
     * @return A copied immutable Config instance
     */
    public static ImmutableCsvProjectTableScanRule.Config copyOf(CsvProjectTableScanRule.Config instance) {
      if (instance instanceof ImmutableCsvProjectTableScanRule.Config) {
        return (ImmutableCsvProjectTableScanRule.Config) instance;
      }
      return ImmutableCsvProjectTableScanRule.Config.builder()
          .from(instance)
          .build();
    }

    /**
     * Creates a builder for {@link ImmutableCsvProjectTableScanRule.Config Config}.
     * <pre>
     * ImmutableCsvProjectTableScanRule.Config.builder()
     *    .withRelBuilderFactory(org.apache.calcite.tools.RelBuilderFactory) // optional {@link CsvProjectTableScanRule.Config#relBuilderFactory() relBuilderFactory}
     *    .withDescription(String | null) // nullable {@link CsvProjectTableScanRule.Config#description() description}
     *    .withOperandSupplier(org.apache.calcite.plan.RelRule.OperandTransform) // optional {@link CsvProjectTableScanRule.Config#operandSupplier() operandSupplier}
     *    .build();
     * </pre>
     * @return A new Config builder
     */
    public static ImmutableCsvProjectTableScanRule.Config.Builder builder() {
      return new ImmutableCsvProjectTableScanRule.Config.Builder();
    }

    /**
     * Builds instances of type {@link ImmutableCsvProjectTableScanRule.Config Config}.
     * Initialize attributes and then invoke the {@link #build()} method to create an
     * immutable instance.
     * <p><em>{@code Builder} is not thread-safe and generally should not be stored in a field or collection,
     * but instead used immediately to create instances.</em>
     */
    @Generated(from = "CsvProjectTableScanRule.Config", generator = "Immutables")
    @NotThreadSafe
    public static final class Builder {
      private @Nullable RelBuilderFactory relBuilderFactory;
      private @Nullable String description;
      private @Nullable RelRule.OperandTransform operandSupplier;

      private Builder() {
      }

      /**
       * Fill a builder with attribute values from the provided {@code org.apache.calcite.plan.RelRule.Config} instance.
       * @param instance The instance from which to copy values
       * @return {@code this} builder for use in a chained invocation
       */
      @CanIgnoreReturnValue 
      public final Builder from(RelRule.Config instance) {
        Objects.requireNonNull(instance, "instance");
        from((Object) instance);
        return this;
      }

      /**
       * Fill a builder with attribute values from the provided {@code org.apache.calcite.adapter.file.CsvProjectTableScanRule.Config} instance.
       * @param instance The instance from which to copy values
       * @return {@code this} builder for use in a chained invocation
       */
      @CanIgnoreReturnValue 
      public final Builder from(CsvProjectTableScanRule.Config instance) {
        Objects.requireNonNull(instance, "instance");
        from((Object) instance);
        return this;
      }

      private void from(Object object) {
        if (object instanceof RelRule.Config) {
          RelRule.Config instance = (RelRule.Config) object;
          withRelBuilderFactory(instance.relBuilderFactory());
          withOperandSupplier(instance.operandSupplier());
          @Nullable String descriptionValue = instance.description();
          if (descriptionValue != null) {
            withDescription(descriptionValue);
          }
        }
      }

      /**
       * Initializes the value for the {@link CsvProjectTableScanRule.Config#relBuilderFactory() relBuilderFactory} attribute.
       * <p><em>If not set, this attribute will have a default value as returned by the initializer of {@link CsvProjectTableScanRule.Config#relBuilderFactory() relBuilderFactory}.</em>
       * @param relBuilderFactory The value for relBuilderFactory 
       * @return {@code this} builder for use in a chained invocation
       */
      @CanIgnoreReturnValue 
      public final Builder withRelBuilderFactory(RelBuilderFactory relBuilderFactory) {
        this.relBuilderFactory = Objects.requireNonNull(relBuilderFactory, "relBuilderFactory");
        return this;
      }

      /**
       * Initializes the value for the {@link CsvProjectTableScanRule.Config#description() description} attribute.
       * @param description The value for description (can be {@code null})
       * @return {@code this} builder for use in a chained invocation
       */
      @CanIgnoreReturnValue 
      public final Builder withDescription(@Nullable String description) {
        this.description = description;
        return this;
      }

      /**
       * Initializes the value for the {@link CsvProjectTableScanRule.Config#operandSupplier() operandSupplier} attribute.
       * <p><em>If not set, this attribute will have a default value as returned by the initializer of {@link CsvProjectTableScanRule.Config#operandSupplier() operandSupplier}.</em>
       * @param operandSupplier The value for operandSupplier 
       * @return {@code this} builder for use in a chained invocation
       */
      @CanIgnoreReturnValue 
      public final Builder withOperandSupplier(RelRule.OperandTransform operandSupplier) {
        this.operandSupplier = Objects.requireNonNull(operandSupplier, "operandSupplier");
        return this;
      }

      /**
       * Builds a new {@link ImmutableCsvProjectTableScanRule.Config Config}.
       * @return An immutable instance of Config
       * @throws java.lang.IllegalStateException if any required attributes are missing
       */
      public ImmutableCsvProjectTableScanRule.Config build() {
        return new ImmutableCsvProjectTableScanRule.Config(this);
      }
    }
  }
}
